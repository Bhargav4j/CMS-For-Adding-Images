@echo off
setlocal enabledelayedexpansion

echo ========================================
echo AWS ECS Fargate Deployment Script
echo WebGallery Application
echo ========================================
echo.

set PROJECT_NAME=webgallery
set TASK_DEFINITION_FILE=ecs\task-definition.json
set SERVICE_DEFINITION_FILE=ecs\service-definition.json

if not exist "!TASK_DEFINITION_FILE!" (
    echo ERROR: Task definition file not found: !TASK_DEFINITION_FILE!
    exit /b 1
)

if not exist "!SERVICE_DEFINITION_FILE!" (
    echo ERROR: Service definition file not found: !SERVICE_DEFINITION_FILE!
    exit /b 1
)

set /p AWS_REGION="Enter AWS Region (e.g., us-east-1): "
set /p CLUSTER_NAME="Enter ECS Cluster Name (e.g., webgallery-cluster): "
set /p IMAGE_URI="Enter Docker Image URI (e.g., 123456789.dkr.ecr.us-east-1.amazonaws.com/webgallery:latest): "

echo.
echo --- Network Configuration ---
set /p VPC_ID="Enter VPC ID (e.g., vpc-0abc123def456): "
set /p SUBNETS_INPUT="Enter Subnet IDs comma-separated (e.g., subnet-0abc123,subnet-0def456): "
set /p SECURITY_GROUP="Enter Security Group ID (e.g., sg-0abc123def): "

for /f "tokens=1,2 delims=," %%a in ("!SUBNETS_INPUT!") do (
    set SUBNET_1=%%a
    set SUBNET_2=%%b
)
if "!SUBNET_2!"=="" set SUBNET_2=!SUBNET_1!

echo.
echo --- Database Configuration ---
set /p DB_SERVER="Enter Database Server Endpoint (e.g., mydb.abc123.us-east-1.rds.amazonaws.com): "
set /p DB_USER="Enter Database Username (default: sa): "
if "!DB_USER!"=="" set DB_USER=sa
set /p DB_PASSWORD="Enter Database Password: "

echo.
set /p NEED_LB="Do you need a load balancer for this service? (y/n): "

if /i "!NEED_LB!"=="y" (
    echo.
    echo Creating Application Load Balancer and Target Group...
    
    set LB_NAME=!PROJECT_NAME!-alb
    set TG_NAME=!PROJECT_NAME!-tg
    
    echo Creating Application Load Balancer: !LB_NAME!
    for /f "delims=" %%i in ('aws elbv2 create-load-balancer --name !LB_NAME! --subnets !SUBNET_1! !SUBNET_2! --security-groups !SECURITY_GROUP! --scheme internet-facing --type application --ip-address-type ipv4 --region !AWS_REGION! --query "LoadBalancers[0].LoadBalancerArn" --output text') do set LB_ARN=%%i
    
    if "!LB_ARN!"=="" (
        echo ERROR: Failed to create load balancer
        exit /b 1
    )
    
    echo Load Balancer ARN: !LB_ARN!
    
    echo Creating Target Group: !TG_NAME!
    for /f "delims=" %%i in ('aws elbv2 create-target-group --name !TG_NAME! --protocol HTTP --port 8080 --vpc-id !VPC_ID! --target-type ip --health-check-enabled --health-check-protocol HTTP --health-check-path /health --health-check-interval-seconds 30 --health-check-timeout-seconds 5 --healthy-threshold-count 2 --unhealthy-threshold-count 3 --region !AWS_REGION! --query "TargetGroups[0].TargetGroupArn" --output text') do set TARGET_GROUP_ARN=%%i
    
    if "!TARGET_GROUP_ARN!"=="" (
        echo ERROR: Failed to create target group
        exit /b 1
    )
    
    echo Target Group ARN: !TARGET_GROUP_ARN!
    
    echo Creating Listener on port 80...
    aws elbv2 create-listener --load-balancer-arn !LB_ARN! --protocol HTTP --port 80 --default-actions Type=forward,TargetGroupArn=!TARGET_GROUP_ARN! --region !AWS_REGION! >nul
    
    echo Load Balancer and Target Group created successfully
    
    for /f "delims=" %%i in ('aws elbv2 describe-load-balancers --load-balancer-arns !LB_ARN! --region !AWS_REGION! --query "LoadBalancers[0].DNSName" --output text') do set LB_DNS=%%i
    
    set USE_LB=true
) else (
    echo Skipping load balancer creation
    set USE_LB=false
    powershell -Command "(Get-Content '!SERVICE_DEFINITION_FILE!' | ConvertFrom-Json | Select-Object -Property * -ExcludeProperty loadBalancers,healthCheckGracePeriodSeconds | ConvertTo-Json -Depth 10) | Set-Content '!SERVICE_DEFINITION_FILE!.tmp'"
    move /y "!SERVICE_DEFINITION_FILE!.tmp" "!SERVICE_DEFINITION_FILE!" >nul
)

echo.
echo Getting AWS Account ID...
for /f "delims=" %%i in ('aws sts get-caller-identity --query Account --output text') do set ACCOUNT_ID=%%i
echo Account ID: !ACCOUNT_ID!

echo.
echo Checking if ECS cluster exists...
aws ecs describe-clusters --clusters !CLUSTER_NAME! --region !AWS_REGION! >nul 2>&1
if !ERRORLEVEL! neq 0 (
    echo Cluster does not exist. Creating ECS cluster: !CLUSTER_NAME!
    aws ecs create-cluster --cluster-name !CLUSTER_NAME! --region !AWS_REGION!
)

echo.
echo Creating CloudWatch Log Group...
aws logs create-log-group --log-group-name "/ecs/!PROJECT_NAME!" --region !AWS_REGION! 2>nul

echo.
echo Replacing placeholders in task definition...
powershell -Command "(Get-Content '!TASK_DEFINITION_FILE!') -replace '{{IMAGE_URI}}','!IMAGE_URI!' -replace '{{AWS_REGION}}','!AWS_REGION!' -replace '{{ACCOUNT_ID}}','!ACCOUNT_ID!' -replace '{{DB_SERVER}}','!DB_SERVER!' -replace '{{DB_USER}}','!DB_USER!' -replace '{{DB_PASSWORD}}','!DB_PASSWORD!' | Set-Content '!TASK_DEFINITION_FILE!.tmp'"

echo Registering ECS task definition...
for /f "delims=" %%i in ('aws ecs register-task-definition --cli-input-json file://!TASK_DEFINITION_FILE!.tmp --region !AWS_REGION! --query "taskDefinition.taskDefinitionArn" --output text') do set TASK_DEF_ARN=%%i

if "!TASK_DEF_ARN!"=="" (
    echo ERROR: Failed to register task definition
    del "!TASK_DEFINITION_FILE!.tmp"
    exit /b 1
)

echo Task Definition ARN: !TASK_DEF_ARN!
del "!TASK_DEFINITION_FILE!.tmp"

echo.
echo Replacing placeholders in service definition...
powershell -Command "(Get-Content '!SERVICE_DEFINITION_FILE!') -replace '{{CLUSTER_NAME}}','!CLUSTER_NAME!' -replace '{{SUBNET_1}}','!SUBNET_1!' -replace '{{SUBNET_2}}','!SUBNET_2!' -replace '{{SECURITY_GROUP}}','!SECURITY_GROUP!' -replace '{{TARGET_GROUP_ARN}}','!TARGET_GROUP_ARN!' | Set-Content '!SERVICE_DEFINITION_FILE!.tmp'"

set SERVICE_NAME=!PROJECT_NAME!-service

echo.
echo Checking if ECS service exists...
for /f "delims=" %%i in ('aws ecs describe-services --cluster !CLUSTER_NAME! --services !SERVICE_NAME! --region !AWS_REGION! --query "services[?status==`ACTIVE`].serviceName" --output text') do set EXISTING_SERVICE=%%i

if "!EXISTING_SERVICE!"=="" (
    echo Service does not exist. Creating new ECS service...
    aws ecs create-service --cli-input-json file://!SERVICE_DEFINITION_FILE!.tmp --region !AWS_REGION!
) else (
    echo Service exists. Updating ECS service...
    aws ecs update-service --cluster !CLUSTER_NAME! --service !SERVICE_NAME! --task-definition !TASK_DEF_ARN! --region !AWS_REGION!
)

del "!SERVICE_DEFINITION_FILE!.tmp"

echo.
echo Waiting for service to become stable...
aws ecs wait services-stable --cluster !CLUSTER_NAME! --services !SERVICE_NAME! --region !AWS_REGION!

echo.
echo ========================================
echo Deployment Complete!
echo ========================================
echo Cluster: !CLUSTER_NAME!
echo Service: !SERVICE_NAME!
echo Task Definition: !TASK_DEF_ARN!

if "!USE_LB!"=="true" (
    echo Load Balancer DNS: http://!LB_DNS!
    echo Access your application at: http://!LB_DNS!
)

echo CloudWatch Logs: /ecs/!PROJECT_NAME!
echo.
echo Verify deployment:
echo aws ecs describe-services --cluster !CLUSTER_NAME! --services !SERVICE_NAME! --region !AWS_REGION!
echo.

endlocal
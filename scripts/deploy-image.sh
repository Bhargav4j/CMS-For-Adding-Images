#!/bin/bash
set -e
set -o pipefail

echo "========================================"
echo "AWS ECS Fargate Deployment Script"
echo "WebGallery Application"
echo "========================================"
echo ""

# Configuration
PROJECT_NAME="webgallery"
TASK_DEFINITION_FILE="ecs/task-definition.json"
SERVICE_DEFINITION_FILE="ecs/service-definition.json"

# Check required files
if [ ! -f "$TASK_DEFINITION_FILE" ]; then
    echo "ERROR: Task definition file not found: $TASK_DEFINITION_FILE"
    exit 1
fi

if [ ! -f "$SERVICE_DEFINITION_FILE" ]; then
    echo "ERROR: Service definition file not found: $SERVICE_DEFINITION_FILE"
    exit 1
fi

# Prompt for AWS configuration
read -p "Enter AWS Region (e.g., us-east-1): " AWS_REGION
read -p "Enter ECS Cluster Name (e.g., webgallery-cluster): " CLUSTER_NAME
read -p "Enter Docker Image URI (e.g., 123456789.dkr.ecr.us-east-1.amazonaws.com/webgallery:latest): " IMAGE_URI

echo ""
echo "--- Network Configuration ---"
read -p "Enter VPC ID (e.g., vpc-0abc123def456): " VPC_ID
read -p "Enter Subnet IDs comma-separated (e.g., subnet-0abc123,subnet-0def456): " SUBNETS_INPUT
read -p "Enter Security Group ID (e.g., sg-0abc123def): " SECURITY_GROUP

# Parse subnets
IFS=',' read -ra SUBNET_ARRAY <<< "$SUBNETS_INPUT"
SUBNET_1=${SUBNET_ARRAY[0]}
SUBNET_2=${SUBNET_ARRAY[1]:-$SUBNET_1}

echo ""
echo "--- Database Configuration ---"
read -p "Enter Database Server Endpoint (e.g., mydb.abc123.us-east-1.rds.amazonaws.com): " DB_SERVER
read -p "Enter Database Username (default: sa): " DB_USER
DB_USER=${DB_USER:-sa}
read -sp "Enter Database Password: " DB_PASSWORD
echo ""

echo ""
read -p "Do you need a load balancer for this service? (y/n): " NEED_LB

if [[ "$NEED_LB" =~ ^[Yy]$ ]]; then
    echo ""
    echo "Creating Application Load Balancer and Target Group..."
    
    LB_NAME="${PROJECT_NAME}-alb"
    TG_NAME="${PROJECT_NAME}-tg"
    
    # Create Application Load Balancer
    echo "Creating Application Load Balancer: $LB_NAME"
    LB_ARN=$(aws elbv2 create-load-balancer \
        --name "$LB_NAME" \
        --subnets $SUBNET_1 $SUBNET_2 \
        --security-groups "$SECURITY_GROUP" \
        --scheme internet-facing \
        --type application \
        --ip-address-type ipv4 \
        --region "$AWS_REGION" \
        --query 'LoadBalancers[0].LoadBalancerArn' \
        --output text)
    
    if [ -z "$LB_ARN" ]; then
        echo "ERROR: Failed to create load balancer"
        exit 1
    fi
    
    echo "Load Balancer ARN: $LB_ARN"
    
    # Create Target Group with target-type ip (required for Fargate awsvpc)
    echo "Creating Target Group: $TG_NAME"
    TARGET_GROUP_ARN=$(aws elbv2 create-target-group \
        --name "$TG_NAME" \
        --protocol HTTP \
        --port 8080 \
        --vpc-id "$VPC_ID" \
        --target-type ip \
        --health-check-enabled \
        --health-check-protocol HTTP \
        --health-check-path /health \
        --health-check-interval-seconds 30 \
        --health-check-timeout-seconds 5 \
        --healthy-threshold-count 2 \
        --unhealthy-threshold-count 3 \
        --region "$AWS_REGION" \
        --query 'TargetGroups[0].TargetGroupArn' \
        --output text)
    
    if [ -z "$TARGET_GROUP_ARN" ]; then
        echo "ERROR: Failed to create target group"
        exit 1
    fi
    
    echo "Target Group ARN: $TARGET_GROUP_ARN"
    
    # Create Listener
    echo "Creating Listener on port 80..."
    aws elbv2 create-listener \
        --load-balancer-arn "$LB_ARN" \
        --protocol HTTP \
        --port 80 \
        --default-actions Type=forward,TargetGroupArn="$TARGET_GROUP_ARN" \
        --region "$AWS_REGION" > /dev/null
    
    echo "Load Balancer and Target Group created successfully"
    
    # Get Load Balancer DNS Name
    LB_DNS=$(aws elbv2 describe-load-balancers \
        --load-balancer-arns "$LB_ARN" \
        --region "$AWS_REGION" \
        --query 'LoadBalancers[0].DNSName' \
        --output text)
    
    USE_LB=true
else
    echo "Skipping load balancer creation"
    USE_LB=false
    # Remove loadBalancers section from service definition
    jq 'del(.loadBalancers, .healthCheckGracePeriodSeconds)' "$SERVICE_DEFINITION_FILE" > "${SERVICE_DEFINITION_FILE}.tmp" && mv "${SERVICE_DEFINITION_FILE}.tmp" "$SERVICE_DEFINITION_FILE"
fi

echo ""
echo "Getting AWS Account ID..."
ACCOUNT_ID=$(aws sts get-caller-identity --query Account --output text)
echo "Account ID: $ACCOUNT_ID"

echo ""
echo "Checking if ECS cluster exists..."
aws ecs describe-clusters --clusters "$CLUSTER_NAME" --region "$AWS_REGION" >/dev/null 2>&1 || {
    echo "Cluster does not exist. Creating ECS cluster: $CLUSTER_NAME"
    aws ecs create-cluster --cluster-name "$CLUSTER_NAME" --region "$AWS_REGION"
}

echo ""
echo "Creating CloudWatch Log Group..."
aws logs create-log-group --log-group-name "/ecs/$PROJECT_NAME" --region "$AWS_REGION" 2>/dev/null || echo "Log group already exists"

echo ""
echo "Replacing placeholders in task definition..."
cp "$TASK_DEFINITION_FILE" "${TASK_DEFINITION_FILE}.tmp"
sed -i "s|{{IMAGE_URI}}|$IMAGE_URI|g" "${TASK_DEFINITION_FILE}.tmp"
sed -i "s|{{AWS_REGION}}|$AWS_REGION|g" "${TASK_DEFINITION_FILE}.tmp"
sed -i "s|{{ACCOUNT_ID}}|$ACCOUNT_ID|g" "${TASK_DEFINITION_FILE}.tmp"
sed -i "s|{{DB_SERVER}}|$DB_SERVER|g" "${TASK_DEFINITION_FILE}.tmp"
sed -i "s|{{DB_USER}}|$DB_USER|g" "${TASK_DEFINITION_FILE}.tmp"
sed -i "s|{{DB_PASSWORD}}|$DB_PASSWORD|g" "${TASK_DEFINITION_FILE}.tmp"

echo "Registering ECS task definition..."
TASK_DEF_ARN=$(aws ecs register-task-definition \
    --cli-input-json file://"${TASK_DEFINITION_FILE}.tmp" \
    --region "$AWS_REGION" \
    --query 'taskDefinition.taskDefinitionArn' \
    --output text)

if [ -z "$TASK_DEF_ARN" ]; then
    echo "ERROR: Failed to register task definition"
    rm "${TASK_DEFINITION_FILE}.tmp"
    exit 1
fi

echo "Task Definition ARN: $TASK_DEF_ARN"
rm "${TASK_DEFINITION_FILE}.tmp"

echo ""
echo "Replacing placeholders in service definition..."
cp "$SERVICE_DEFINITION_FILE" "${SERVICE_DEFINITION_FILE}.tmp"
sed -i "s|{{CLUSTER_NAME}}|$CLUSTER_NAME|g" "${SERVICE_DEFINITION_FILE}.tmp"
sed -i "s|{{SUBNET_1}}|$SUBNET_1|g" "${SERVICE_DEFINITION_FILE}.tmp"
sed -i "s|{{SUBNET_2}}|$SUBNET_2|g" "${SERVICE_DEFINITION_FILE}.tmp"
sed -i "s|{{SECURITY_GROUP}}|$SECURITY_GROUP|g" "${SERVICE_DEFINITION_FILE}.tmp"

if [ "$USE_LB" = true ]; then
    sed -i "s|{{TARGET_GROUP_ARN}}|$TARGET_GROUP_ARN|g" "${SERVICE_DEFINITION_FILE}.tmp"
fi

SERVICE_NAME="${PROJECT_NAME}-service"

echo ""
echo "Checking if ECS service exists..."
EXISTING_SERVICE=$(aws ecs describe-services \
    --cluster "$CLUSTER_NAME" \
    --services "$SERVICE_NAME" \
    --region "$AWS_REGION" \
    --query 'services[?status==`ACTIVE`].serviceName' \
    --output text)

if [ -z "$EXISTING_SERVICE" ] || [ "$EXISTING_SERVICE" = "None" ]; then
    echo "Service does not exist. Creating new ECS service..."
    aws ecs create-service \
        --cli-input-json file://"${SERVICE_DEFINITION_FILE}.tmp" \
        --region "$AWS_REGION"
else
    echo "Service exists. Updating ECS service..."
    aws ecs update-service \
        --cluster "$CLUSTER_NAME" \
        --service "$SERVICE_NAME" \
        --task-definition "$TASK_DEF_ARN" \
        --region "$AWS_REGION"
fi

rm "${SERVICE_DEFINITION_FILE}.tmp"

echo ""
echo "Waiting for service to become stable..."
aws ecs wait services-stable \
    --cluster "$CLUSTER_NAME" \
    --services "$SERVICE_NAME" \
    --region "$AWS_REGION"

echo ""
echo "========================================"
echo "Deployment Complete!"
echo "========================================"
echo "Cluster: $CLUSTER_NAME"
echo "Service: $SERVICE_NAME"
echo "Task Definition: $TASK_DEF_ARN"

if [ "$USE_LB" = true ]; then
    echo "Load Balancer DNS: http://$LB_DNS"
    echo "Access your application at: http://$LB_DNS"
fi

echo "CloudWatch Logs: /ecs/$PROJECT_NAME"
echo ""
echo "Verify deployment:"
echo "aws ecs describe-services --cluster $CLUSTER_NAME --services $SERVICE_NAME --region $AWS_REGION"
echo ""
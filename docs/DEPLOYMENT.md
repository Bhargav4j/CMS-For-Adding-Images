# WebGallery - AWS ECS Fargate Deployment Guide

## Table of Contents
1. [Prerequisites](#prerequisites)
2. [Project Overview](#project-overview)
3. [Local Development Setup](#local-development-setup)
4. [Docker Deployment](#docker-deployment)
5. [AWS ECS Fargate Prerequisites](#aws-ecs-fargate-prerequisites)
6. [ECS Fargate Setup](#ecs-fargate-setup)
7. [ECS Task Definition Explained](#ecs-task-definition-explained)
8. [ECS Service Configuration](#ecs-service-configuration)
9. [ECS Fargate Deployment Walkthrough](#ecs-fargate-deployment-walkthrough)
10. [ECS-Specific Troubleshooting](#ecs-specific-troubleshooting)
11. [ECS Fargate Scaling and Management](#ecs-fargate-scaling-and-management)
12. [Security Considerations](#security-considerations)
13. [Monitoring and Logging](#monitoring-and-logging)

---

## Prerequisites

### Required Tools
- **.NET 8.0 SDK** - Download from [dotnet.microsoft.com](https://dotnet.microsoft.com/download/dotnet/8.0)
- **Docker Desktop** - For containerization ([docker.com](https://www.docker.com/products/docker-desktop))
- **AWS CLI v2** - For AWS deployment ([docs.aws.amazon.com/cli](https://docs.aws.amazon.com/cli/latest/userguide/install-cliv2.html))
- **jq** (Linux/macOS) - For JSON processing in shell scripts
- **Git** - For version control

### AWS Account Requirements
- Active AWS account with ECS permissions
- IAM user with programmatic access
- AWS CLI configured with credentials (`aws configure`)

### Knowledge Requirements
- Basic understanding of .NET Core and ASP.NET Core
- Familiarity with Docker containers
- Basic AWS ECS concepts (tasks, services, clusters)
- Understanding of SQL Server databases

---

## Project Overview

### Application Details
- **Name**: WebGallery
- **Technology**: ASP.NET Core 8.0 (Razor Pages)
- **Architecture**: Clean Architecture with Domain, Application, Infrastructure, and Web layers
- **Database**: SQL Server with Entity Framework Core
- **Authentication**: ASP.NET Core Identity
- **Logging**: Serilog with Console sink
- **Image Processing**: SixLabors.ImageSharp

### Application Structure
```
WebGallery/
├── src/
│   ├── WebGallery.Web/              # Presentation layer (Razor Pages)
│   ├── WebGallery.Application/       # Application services and business logic
│   ├── WebGallery.Domain/            # Domain entities and interfaces
│   └── WebGallery.Infrastructure/    # Data access and external services
├── tests/
│   ├── WebGallery.Web.Tests/
│   ├── WebGallery.Application.Tests/
│   ├── WebGallery.Domain.Tests/
│   └── WebGallery.Infrastructure.Tests/
├── Dockerfile
├── docker-compose.yml
├── ecs/
│   ├── task-definition.json
│   └── service-definition.json
└── scripts/
    ├── build-push.sh
    ├── build-push.bat
    ├── deploy-image.sh
    └── deploy-image.bat
```

### Application Port
- **Container Port**: 8080 (HTTP)
- **Protocol**: HTTP (HTTPS termination at ALB if needed)

---

## Local Development Setup

### 1. Clone the Repository
```bash
git clone <repository-url>
cd WebGallery
```

### 2. Restore Dependencies
```bash
dotnet restore
```

### 3. Set Up Database
The application uses SQL Server. For local development:

```bash
# Using Docker
docker run -e "ACCEPT_EULA=Y" -e "SA_PASSWORD=YourStrong@Passw0rd" \
  -p 1433:1433 --name sqlserver \
  -d mcr.microsoft.com/mssql/server:2022-latest
```

### 4. Configure Connection String
Update `appsettings.Development.json`:
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost;Database=WebGallery;User Id=sa;Password=YourStrong@Passw0rd;TrustServerCertificate=True"
  }
}
```

### 5. Run Database Migrations
```bash
cd src/WebGallery.Web
dotnet ef database update
```

### 6. Run the Application
```bash
dotnet run --project src/WebGallery.Web
```

Access the application at `http://localhost:5000` or `https://localhost:5001`

---

## Docker Deployment

### Build Docker Image Locally
```bash
docker build -t webgallery:latest -f Dockerfile .
```

### Run with Docker Compose
```bash
# Set environment variables
export DB_SERVER=your-db-server
export DB_PASSWORD=your-password

# Start the application
docker-compose up -d
```

### Access Application
- Application: `http://localhost:8080`

---

## AWS ECS Fargate Prerequisites

### 1. AWS Account Setup
Ensure your AWS CLI is configured:
```bash
aws configure
# Enter: AWS Access Key ID, Secret Access Key, Region, Output format
```

Verify configuration:
```bash
aws sts get-caller-identity
```

### 2. Required IAM Roles

#### ECS Task Execution Role
This role allows ECS to pull images from ECR and send logs to CloudWatch.

```bash
# Create role (if not exists)
aws iam create-role \
  --role-name ecsTaskExecutionRole \
  --assume-role-policy-document '{
    "Version": "2012-10-17",
    "Statement": [{
      "Effect": "Allow",
      "Principal": {"Service": "ecs-tasks.amazonaws.com"},
      "Action": "sts:AssumeRole"
    }]
  }'

# Attach AWS managed policy
aws iam attach-role-policy \
  --role-name ecsTaskExecutionRole \
  --policy-arn arn:aws:iam::aws:policy/service-role/AmazonECSTaskExecutionRolePolicy
```

#### ECS Task Role (Optional)
For application-specific AWS service access (S3, DynamoDB, etc.):

```bash
aws iam create-role \
  --role-name ecsTaskRole \
  --assume-role-policy-document '{
    "Version": "2012-10-17",
    "Statement": [{
      "Effect": "Allow",
      "Principal": {"Service": "ecs-tasks.amazonaws.com"},
      "Action": "sts:AssumeRole"
    }]
  }'
```

### 3. Network Setup

#### VPC and Subnets
ECS Fargate requires:
- VPC with at least 2 subnets in different Availability Zones
- Subnets must have internet access (Internet Gateway or NAT Gateway)
- Public IP assignment enabled (or NAT Gateway for private subnets)

```bash
# List your VPCs
aws ec2 describe-vpcs --query 'Vpcs[*].[VpcId,CidrBlock,Tags[?Key==`Name`].Value|[0]]' --output table

# List subnets in a VPC
aws ec2 describe-subnets --filters "Name=vpc-id,Values=vpc-xxxxx" \
  --query 'Subnets[*].[SubnetId,AvailabilityZone,CidrBlock]' --output table
```

#### Security Group
Create a security group for ECS tasks:

```bash
# Create security group
SG_ID=$(aws ec2 create-security-group \
  --group-name webgallery-ecs-sg \
  --description "Security group for WebGallery ECS tasks" \
  --vpc-id vpc-xxxxx \
  --query 'GroupId' --output text)

# Allow inbound traffic on port 8080 (application port)
aws ec2 authorize-security-group-ingress \
  --group-id $SG_ID \
  --protocol tcp \
  --port 8080 \
  --cidr 0.0.0.0/0

# Allow outbound traffic (default: all traffic allowed)
```

### 4. Database Setup (Amazon RDS)
For production, use Amazon RDS for SQL Server:

```bash
# Create RDS SQL Server instance (example)
aws rds create-db-instance \
  --db-instance-identifier webgallery-db \
  --db-instance-class db.t3.medium \
  --engine sqlserver-ex \
  --master-username admin \
  --master-user-password YourStrong@Passw0rd123 \
  --allocated-storage 20 \
  --vpc-security-group-ids sg-xxxxx \
  --db-subnet-group-name default \
  --backup-retention-period 7 \
  --publicly-accessible
```

**Note**: Ensure the RDS security group allows inbound traffic from the ECS security group on port 1433.

---

## ECS Fargate Setup

### 1. Create ECR Repository
```bash
# Create ECR repository
aws ecr create-repository \
  --repository-name webgallery \
  --region us-east-1

# Get repository URI
REPO_URI=$(aws ecr describe-repositories \
  --repository-names webgallery \
  --query 'repositories[0].repositoryUri' \
  --output text)

echo "ECR Repository URI: $REPO_URI"
```

### 2. Create CloudWatch Log Group
```bash
aws logs create-log-group \
  --log-group-name /ecs/webgallery \
  --region us-east-1
```

### 3. Create ECS Cluster
```bash
aws ecs create-cluster \
  --cluster-name webgallery-cluster \
  --region us-east-1
```

---

## ECS Task Definition Explained

### Key Components

#### 1. Launch Type and Compatibility
```json
"requiresCompatibilities": ["FARGATE"],
"networkMode": "awsvpc"
```
- **FARGATE**: Serverless compute for containers (no EC2 instance management)
- **awsvpc**: Each task gets its own ENI with private IP

#### 2. CPU and Memory
```json
"cpu": "512",
"memory": "1024"
```

**Valid Fargate CPU/Memory Combinations**:
| CPU (vCPU) | Memory (MB) |
|------------|-------------|
| 256 (.25)  | 512, 1024, 2048 |
| 512 (.5)   | 1024, 2048, 3072, 4096 |
| 1024 (1)   | 2048-8192 (1GB increments) |
| 2048 (2)   | 4096-16384 (1GB increments) |
| 4096 (4)   | 8192-30720 (1GB increments) |

**Recommended for .NET Applications**:
- Development: cpu: "512", memory: "1024"
- Production: cpu: "1024", memory: "2048" or higher

#### 3. Execution Role
```json
"executionRoleArn": "arn:aws:iam::{{ACCOUNT_ID}}:role/ecsTaskExecutionRole"
```
- Allows ECS to pull images from ECR
- Enables CloudWatch log writing
- Required for Fargate

#### 4. Container Definition
```json
"containerDefinitions": [
  {
    "name": "webgallery",
    "image": "{{IMAGE_URI}}",
    "essential": true,
    "portMappings": [{"containerPort": 8080, "protocol": "tcp"}],
    "environment": [...],
    "logConfiguration": {...}
  }
]
```

#### 5. Logging Configuration
```json
"logConfiguration": {
  "logDriver": "awslogs",
  "options": {
    "awslogs-group": "/ecs/webgallery",
    "awslogs-region": "us-east-1",
    "awslogs-stream-prefix": "ecs",
    "awslogs-create-group": "true"
  }
}
```

---

## ECS Service Configuration

### Key Components

#### 1. Launch Type and Desired Count
```json
"launchType": "FARGATE",
"desiredCount": 2
```
- Runs 2 tasks for high availability
- Auto-recovers failed tasks

#### 2. Network Configuration (awsvpc)
```json
"networkConfiguration": {
  "awsvpcConfiguration": {
    "subnets": ["subnet-xxx", "subnet-yyy"],
    "securityGroups": ["sg-xxx"],
    "assignPublicIp": "ENABLED"
  }
}
```

**Important Notes**:
- Use **public subnets** with `assignPublicIp: ENABLED` OR
- Use **private subnets** with NAT Gateway (better security)
- Subnets must be in different Availability Zones for HA

#### 3. Deployment Configuration
```json
"deploymentConfiguration": {
  "maximumPercent": 200,
  "minimumHealthyPercent": 50
}
```
- Allows rolling updates without downtime
- Ensures at least 50% capacity during deployment

#### 4. Load Balancer Integration
```json
"loadBalancers": [{
  "targetGroupArn": "arn:aws:elasticloadbalancing:...",
  "containerName": "webgallery",
  "containerPort": 8080
}],
"healthCheckGracePeriodSeconds": 300
```

---

## ECS Fargate Deployment Walkthrough

### Step 1: Build and Push Docker Image

#### Linux/macOS:
```bash
cd /path/to/WebGallery
chmod +x scripts/build-push.sh
./scripts/build-push.sh
```

#### Windows:
```cmd
cd C:\path\to\WebGallery
scripts\build-push.bat
```

**Script Workflow**:
1. Prompts for registry type (ECR or Docker Hub)
2. Prompts for registry credentials
3. Sanitizes image name and tag
4. Builds Docker image using multi-stage Dockerfile
5. Pushes image to selected registry
6. Displays image URI for deployment

**Example Output**:
```
Image: 123456789.dkr.ecr.us-east-1.amazonaws.com/webgallery:latest
```

### Step 2: Deploy to ECS Fargate

#### Linux/macOS:
```bash
chmod +x scripts/deploy-image.sh
./scripts/deploy-image.sh
```

#### Windows:
```cmd
scripts\deploy-image.bat
```

**Script Workflow**:
1. Prompts for AWS region and ECS cluster name
2. Prompts for Docker image URI (from Step 1)
3. Prompts for network configuration:
   - VPC ID
   - Subnet IDs (comma-separated)
   - Security Group ID
4. Prompts for database configuration:
   - Database server endpoint (RDS endpoint)
   - Database username and password
5. Asks if load balancer is needed:
   - If **Yes**: Automatically creates ALB and Target Group
   - If **No**: Deploys without load balancer
6. Replaces placeholders in JSON files
7. Registers ECS task definition
8. Creates or updates ECS service
9. Waits for service stability
10. Displays deployment status and URLs

**Example Prompts**:
```
Enter AWS Region (e.g., us-east-1): us-east-1
Enter ECS Cluster Name (e.g., webgallery-cluster): webgallery-cluster
Enter Docker Image URI: 123456789.dkr.ecr.us-east-1.amazonaws.com/webgallery:latest
Enter VPC ID (e.g., vpc-0abc123def456): vpc-0abc123
Enter Subnet IDs comma-separated: subnet-0abc123,subnet-0def456
Enter Security Group ID: sg-0abc123
Enter Database Server Endpoint: webgallery-db.abc123.us-east-1.rds.amazonaws.com
Enter Database Username (default: sa): admin
Enter Database Password: ********
Do you need a load balancer for this service? (y/n): y
```

### Step 3: Verify Deployment

```bash
# Check service status
aws ecs describe-services \
  --cluster webgallery-cluster \
  --services webgallery-service \
  --region us-east-1

# List running tasks
aws ecs list-tasks \
  --cluster webgallery-cluster \
  --service-name webgallery-service \
  --region us-east-1

# View logs
aws logs tail /ecs/webgallery --follow --region us-east-1
```

### Step 4: Access Application

**With Load Balancer**:
- Access via ALB DNS name: `http://webgallery-alb-xxxxx.us-east-1.elb.amazonaws.com`

**Without Load Balancer**:
- Get task public IP:
  ```bash
  aws ecs describe-tasks \
    --cluster webgallery-cluster \
    --tasks <task-arn> \
    --region us-east-1 \
    --query 'tasks[0].attachments[0].details[?name==`networkInterfaceId`].value' \
    --output text
  
  # Get public IP from ENI
  aws ec2 describe-network-interfaces \
    --network-interface-ids <eni-id> \
    --query 'NetworkInterfaces[0].Association.PublicIp' \
    --output text
  ```
- Access via public IP: `http://<public-ip>:8080`

---

## ECS-Specific Troubleshooting

### Task Fails to Start

**1. Check Task Status**:
```bash
aws ecs describe-tasks \
  --cluster webgallery-cluster \
  --tasks <task-arn> \
  --region us-east-1
```

**Common Issues**:
- **"CannotPullContainerError"**: ECR permissions issue
  - Verify `ecsTaskExecutionRole` has ECR pull permissions
  - Check image URI is correct
- **"ResourceInitializationError"**: CPU/memory invalid combination
  - Use valid Fargate CPU/memory pairs
- **"Task failed to start"**: Container crashes on startup
  - Check CloudWatch logs for application errors

### Database Connection Issues

**Symptoms**: Task starts but crashes immediately

**Solutions**:
1. Verify RDS security group allows traffic from ECS security group on port 1433
2. Check database endpoint is correct
3. Verify database credentials
4. Check CloudWatch logs:
   ```bash
   aws logs tail /ecs/webgallery --follow --region us-east-1
   ```

### Network Configuration Issues

**Symptoms**: Task fails to start with network errors

**Solutions**:
1. Ensure subnets have internet access (Internet Gateway or NAT Gateway)
2. Verify security group allows outbound traffic
3. If using `assignPublicIp: ENABLED`, use public subnets
4. Check route tables for subnet

### CPU/Memory Errors

**Error**: "Invalid CPU or memory value specified"

**Solution**: Use valid Fargate combinations (see [ECS Task Definition](#ecs-task-definition-explained))

### Load Balancer Health Check Failures

**Symptoms**: Tasks start but are marked unhealthy

**Solutions**:
1. Verify application is listening on port 8080
2. Check health check path is accessible: `/health`
3. Increase `healthCheckGracePeriodSeconds` in service definition (default: 300)
4. Verify target group health check settings:
   ```bash
   aws elbv2 describe-target-health \
     --target-group-arn <target-group-arn> \
     --region us-east-1
   ```

---

## ECS Fargate Scaling and Management

### Manual Scaling

```bash
# Scale to 4 tasks
aws ecs update-service \
  --cluster webgallery-cluster \
  --service webgallery-service \
  --desired-count 4 \
  --region us-east-1
```

### Auto Scaling (Recommended)

#### 1. Register Scalable Target
```bash
aws application-autoscaling register-scalable-target \
  --service-namespace ecs \
  --resource-id service/webgallery-cluster/webgallery-service \
  --scalable-dimension ecs:service:DesiredCount \
  --min-capacity 2 \
  --max-capacity 10 \
  --region us-east-1
```

#### 2. Create Scaling Policy (Target Tracking)
```bash
# Scale based on CPU utilization
aws application-autoscaling put-scaling-policy \
  --service-namespace ecs \
  --resource-id service/webgallery-cluster/webgallery-service \
  --scalable-dimension ecs:service:DesiredCount \
  --policy-name cpu-scaling-policy \
  --policy-type TargetTrackingScaling \
  --target-tracking-scaling-policy-configuration '{
    "TargetValue": 70.0,
    "PredefinedMetricSpecification": {
      "PredefinedMetricType": "ECSServiceAverageCPUUtilization"
    },
    "ScaleInCooldown": 300,
    "ScaleOutCooldown": 60
  }' \
  --region us-east-1
```

### Blue/Green Deployments

For zero-downtime deployments with rollback capability:

1. Use AWS CodeDeploy with ECS
2. Configure deployment group with blue/green settings
3. Deploy new task definition version
4. CodeDeploy automatically handles traffic shifting

---

## Security Considerations

### 1. Network Security
- **Use private subnets** with NAT Gateway for production
- **Restrict security group** rules to minimum required ports
- **Enable VPC Flow Logs** for network monitoring

### 2. Container Security
- **Run as non-root user** (implemented in Dockerfile)
- **Scan images** for vulnerabilities using ECR image scanning:
  ```bash
  aws ecr start-image-scan \
    --repository-name webgallery \
    --image-id imageTag=latest \
    --region us-east-1
  ```
- **Use secrets management** for sensitive data:
  - Store database passwords in AWS Secrets Manager
  - Reference secrets in task definition:
    ```json
    "secrets": [
      {
        "name": "DB_PASSWORD",
        "valueFrom": "arn:aws:secretsmanager:region:account-id:secret:secret-name"
      }
    ]
    ```

### 3. IAM Security
- **Least privilege**: Grant only necessary permissions to task role
- **Separate roles**: Use different task roles for different services
- **Avoid hardcoded credentials**: Use IAM roles for AWS service access

### 4. Database Security
- **Enable encryption** at rest and in transit for RDS
- **Use private subnets** for RDS instances
- **Rotate credentials** regularly using Secrets Manager
- **Enable automated backups** and point-in-time recovery

---

## Monitoring and Logging

### CloudWatch Logs

View application logs:
```bash
# Tail logs in real-time
aws logs tail /ecs/webgallery --follow --region us-east-1

# Query logs
aws logs filter-log-events \
  --log-group-name /ecs/webgallery \
  --filter-pattern "ERROR" \
  --region us-east-1
```

### CloudWatch Metrics

Monitor key ECS metrics:
- **CPUUtilization**: Average CPU usage across tasks
- **MemoryUtilization**: Average memory usage across tasks
- **TargetResponseTime**: Application response time (ALB)
- **HealthyHostCount**: Number of healthy targets (ALB)

### CloudWatch Alarms

```bash
# Create alarm for high CPU
aws cloudwatch put-metric-alarm \
  --alarm-name webgallery-high-cpu \
  --alarm-description "Alert when CPU exceeds 80%" \
  --metric-name CPUUtilization \
  --namespace AWS/ECS \
  --statistic Average \
  --period 300 \
  --threshold 80 \
  --comparison-operator GreaterThanThreshold \
  --evaluation-periods 2 \
  --dimensions Name=ServiceName,Value=webgallery-service Name=ClusterName,Value=webgallery-cluster \
  --region us-east-1
```

### Application Insights (Optional)

For production applications, consider:
- **AWS X-Ray**: Distributed tracing
- **Application Insights for .NET**: Deep application monitoring
- **Third-party APM**: New Relic, Datadog, Dynatrace

---

## Additional Resources

### AWS Documentation
- [Amazon ECS Developer Guide](https://docs.aws.amazon.com/AmazonECS/latest/developerguide/)
- [AWS Fargate Documentation](https://docs.aws.amazon.com/AmazonECS/latest/developerguide/AWS_Fargate.html)
- [ECS Task Definition Parameters](https://docs.aws.amazon.com/AmazonECS/latest/developerguide/task_definition_parameters.html)

### .NET on AWS
- [.NET on AWS](https://aws.amazon.com/developer/language/net/)
- [AWS SDK for .NET](https://aws.amazon.com/sdk-for-net/)
- [ASP.NET Core on ECS](https://docs.aws.amazon.com/AmazonECS/latest/developerguide/create-container-image.html)

### Best Practices
- [ECS Best Practices Guide](https://docs.aws.amazon.com/AmazonECS/latest/bestpracticesguide/intro.html)
- [Docker Best Practices](https://docs.docker.com/develop/dev-best-practices/)
- [ASP.NET Core Performance Best Practices](https://docs.microsoft.com/en-us/aspnet/core/performance/performance-best-practices)

---

## Support and Troubleshooting

For issues or questions:
1. Check CloudWatch logs: `aws logs tail /ecs/webgallery --follow`
2. Review ECS service events: `aws ecs describe-services --cluster webgallery-cluster --services webgallery-service`
3. Verify task definition: `aws ecs describe-task-definition --task-definition webgallery-task`
4. Check AWS Health Dashboard for service issues

---

**Deployment Guide Version**: 1.0  
**Last Updated**: 2026-01-07  
**Target Platform**: AWS ECS Fargate  
**Application Version**: WebGallery 1.0 (.NET 8.0)
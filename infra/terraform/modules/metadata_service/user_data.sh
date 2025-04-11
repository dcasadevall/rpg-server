#!/bin/bash
set -ex

# Install Docker
sudo snap install docker
systemctl enable docker
usermod -aG docker ubuntu

# Login to ECR
aws ecr get-login-password --region ${region} | docker login --username AWS --password-stdin ${ecr_registry}

# Pull and run the metadata service container
docker pull ${metadata_repository_url}:latest
docker run -d \
  -e ENVIRONMENT=${environment} \
  -e DYNAMODB_DB_PREFIX=${environment}- \
  -e DYNAMODB_SERVICE_URL=dynamodb.${region}.amazonaws.com \
  -p 80:80 \
  ${metadata_repository_url}:latest

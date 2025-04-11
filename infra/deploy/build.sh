#!/bin/bash

# Set variables
IMAGE_NAME="rpg-character-service"
IMAGE_TAG="latest"

# Get ECR repository URL from Terraform output
ECR_REPOSITORY=$(terraform -chdir=../provision output -raw ecr_repository_url)

# Build the Docker image from the root directory
echo "Building Docker image..."
docker build -t ${IMAGE_NAME}:${IMAGE_TAG} -f Dockerfile ../../

# Tag the image for ECR
echo "Tagging image for ECR..."
docker tag ${IMAGE_NAME}:${IMAGE_TAG} ${ECR_REPOSITORY}:${IMAGE_TAG}

# Login to ECR
echo "Logging in to ECR..."
aws ecr get-login-password --region us-east-1 | docker login --username AWS --password-stdin ${ECR_REPOSITORY%/*}

# Push the image to ECR
echo "Pushing image to ECR..."
docker push ${ECR_REPOSITORY}:${IMAGE_TAG}

echo "Done!"

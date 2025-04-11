#!/bin/bash

# Set variables
IMAGE_NAME="metadata-service"
IMAGE_TAG="latest"
ENVIRONMENT=${ENVIRONMENT:-"dev"}

# Get ECR repository URL from Terraform output
ECR_REPOSITORY=$(terraform -chdir=../../provision output -raw ecr_repository_url)

# Build the Docker image
echo "Building metadata service Docker image..."
docker build -t ${IMAGE_NAME}:${IMAGE_TAG} \
    --build-arg ENVIRONMENT=${ENVIRONMENT} \
    -f Dockerfile ../../../

# Tag the image for ECR
echo "Tagging image for ECR..."
docker tag ${IMAGE_NAME}:${IMAGE_TAG} ${ECR_REPOSITORY}:${IMAGE_TAG}

# Login to ECR
echo "Logging in to ECR..."
aws ecr get-login-password --region us-east-1 | docker login --username AWS --password-stdin ${ECR_REPOSITORY%/*}

# Push the image to ECR
echo "Pushing image to ECR..."
docker push ${ECR_REPOSITORY}:${IMAGE_TAG}

# Deploy using Kustomize
echo "Deploying to Kubernetes..."
cd ../overlays/${ENVIRONMENT}
kustomize edit set image ${IMAGE_NAME}=${ECR_REPOSITORY}:${IMAGE_TAG}
kustomize build . | kubectl apply -f -

echo "Metadata service deployment complete!"

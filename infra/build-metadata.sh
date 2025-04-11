#!/bin/bash

# Get the ECR repository URL from Terraform
ECR_REPOSITORY_URL=$(terraform -chdir=terraform output -raw metadata_repository_url)

# Build the Docker image
docker build -t ${ECR_REPOSITORY_URL}:latest -f docker/metadata.Dockerfile ../

# Login to ECR
aws ecr get-login-password --region $(terraform -chdir=terraform output -raw region) | docker login --username AWS --password-stdin ${ECR_REPOSITORY_URL%/*}

# Push the image to ECR
docker push ${ECR_REPOSITORY_URL}:latest

#!/bin/bash

# Get the ECR repository URL from Terraform
ECR_REPOSITORY_URL=$(terraform -chdir=terraform output -raw game_sim_repository_url)

# Check if the gamesim-service directory exists
if [ ! -d "../gamesim-service" ]; then
    echo "Error: gamesim-service directory not found at ../gamesim-service"
    exit 1
fi

# Build the Docker image
docker build --platform linux/amd64 -t ${ECR_REPOSITORY_URL}:latest -f docker/gamesim.Dockerfile ../

# Login to ECR
aws ecr get-login-password --region $(terraform -chdir=terraform output -raw region) | docker login --username AWS --password-stdin ${ECR_REPOSITORY_URL%/*}

# Push the image to ECR
docker push ${ECR_REPOSITORY_URL}:latest

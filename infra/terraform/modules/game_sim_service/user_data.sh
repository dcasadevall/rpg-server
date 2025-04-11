#!/bin/bash
# Install Docker
yum update -y
amazon-linux-extras install docker
service docker start
usermod -a -G docker ec2-user

# Login to ECR
aws ecr get-login-password --region ${region} | docker login --username AWS --password-stdin ${ecr_registry}

# Pull and run the game simulation service container
docker pull ${game_sim_repository_url}:latest
docker run -d \
  -e GAME_SIM_PORT=${udp_port} \
  -e GAME_SIM_ENVIRONMENT=${environment} \
  -e METADATA_SERVICE_URL=https://${metadata_service_dns} \
  -p ${udp_port}:${udp_port}/udp \
  ${game_sim_repository_url}:latest

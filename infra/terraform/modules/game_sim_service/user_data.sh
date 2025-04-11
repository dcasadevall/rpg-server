#!/bin/bash
# Install Docker
sudo snap install docker
sudo snap start docker

# Wait for docker to be ready
until docker info; do sleep 1; done

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

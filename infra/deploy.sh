#!/bin/bash

# Set environment variable
ENVIRONMENT=${ENVIRONMENT:-"dev"}

# Make build scripts executable
chmod +x deploy/metadata_service/deploy-metadata.sh
chmod +x deploy/gamesim_service/deploy-game-sim.sh

# Build and deploy metadata service
echo "Building and deploying metadata service..."
cd deploy/metadata_service
./deploy-metadata.sh
cd ../..

# Build and deploy game simulation service
echo "Building and deploying game simulation service..."
cd deploy/gamesim_service
./deploy-game-sim.sh
cd ../..

echo "All services deployed successfully!"

#!/bin/bash

# Set environment variable
ENVIRONMENT=${ENVIRONMENT:-"dev"}

# Make build scripts executable
chmod +x deploy/metadata_service/build-metadata.sh
chmod +x deploy/gamesim_service/build-game-sim.sh

# Build and deploy metadata service
echo "Building and deploying metadata service..."
cd deploy/metadata_service
./build-metadata.sh
cd ../..

# Build and deploy game simulation service
echo "Building and deploying game simulation service..."
cd deploy/gamesim_service
./build-game-sim.sh
cd ../..

echo "All services deployed successfully!"

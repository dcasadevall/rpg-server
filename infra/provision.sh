#!/bin/bash

# Change to the provision directory
cd "$(dirname "$0")/provision"

# Initialize Terraform
echo "Initializing Terraform..."
terraform init

# Apply Terraform configuration
echo "Applying Terraform configuration..."
terraform apply

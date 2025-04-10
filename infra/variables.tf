variable "region" {
  description = "The AWS region to deploy resources in"
  type        = string
  default     = "us-east-1"
}

variable "iam_username" {
  description = "IAM username to attach the infrastructure management policy to"
  type        = string
  default     = "Dani"
}

variable "certificate_arn" {
  description = "ARN of the SSL certificate to use for HTTPS"
  type        = string
  default     = null
}

# Static Store Variables
variable "static_bucket_name" {
  description = "Name of the S3 bucket for static content"
  type        = string
  default     = "game-static-content"
}

# Metadata Service Variables

# Default AMI ID: Amazon Linux 2023 AMI
# Modern, secure, AWS-supported, ideal for EC2 servers
variable "metadata_ami_id" {
  description = "AMI ID for the metadata service instances"
  type        = string
  default     = "ami-0c2b0c5c0c5c0c5c0"  # Amazon Linux 2023 AMI for us-east-1
}

variable "metadata_instance_type" {
  description = "The EC2 instance type for the metadata service"
  type        = string
  default     = "t2.micro"
}

variable "metadata_user_data" {
  description = "User data script for metadata service instance initialization"
  type        = string
  default     = <<-EOF
    #!/bin/bash
    echo "Metadata service initializing..." > /var/log/metadata-service.log
  EOF
}

# Game Simulation Service Variables
variable "gamesim_udp_port" {
  description = "UDP port for game simulation traffic"
  type        = number
  default     = 7777
}

# Default AMI ID: Amazon Linux 2023 AMI
# Modern, secure, AWS-supported, ideal for EC2 servers
variable "gamesim_ami_id" {
  description = "AMI ID for the game simulation service instances"
  type        = string
  default     = "ami-0c2b0c5c0c5c0c5c0"  # Amazon Linux 2023 AMI for us-east-1
}

variable "gamesim_instance_type" {
  description = "The EC2 instance type for the game simulation service"
  type        = string
  default     = "c5.large"
}

variable "gamesim_user_data" {
  description = "User data script for game simulation instance initialization"
  type        = string
  default     = <<-EOF
    #!/bin/bash
    echo "Starting game simulation server..." > /var/log/game-sim.log
  EOF
}

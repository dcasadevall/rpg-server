variable "environment" {
  description = "Environment name (e.g., dev, prod)"
  type        = string
  default     = "dev"
}

variable "project_name" {
  description = "Name of the project used for resource naming and tagging"
  type        = string
  default     = "rpg-game"
}

variable "region" {
  description = "The AWS region to deploy resources in"
  type        = string
  default     = "us-east-1"
}

variable "domain_name" {
  description = "Domain name for the application"
  type        = string
  default     = "dcasadevall.com"
}

variable "instance_type" {
  description = "Default EC2 instance type for services"
  type        = string
  default     = "t2.micro"
}

variable "bucket_name" {
  description = "Name of the S3 bucket for static content"
  type        = string
  default     = "rpg-game-static-content"
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
  description = "AMI ID for metadata service instances"
  type        = string
  default     = "ami-0c7217cdde317cfec"  # Amazon Linux 2023 in us-east-1
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

variable "metadata_desired_capacity" {
  description = "Desired number of metadata service tasks"
  type        = number
  default     = 1
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
  description = "AMI ID for game simulation service instances"
  type        = string
  default     = "ami-0c7217cdde317cfec"  # Amazon Linux 2023 in us-east-1
}

variable "gamesim_instance_type" {
  description = "The EC2 instance type for the game simulation service"
  type        = string
  default     = "c5.large"
}

variable "tags" {
  description = "A map of tags to add to all resources"
  type        = map(string)
  default     = {}
}

variable "game_sim_instance_type" {
  description = "EC2 instance type for game simulation service"
  type        = string
  default     = "t2.micro"
}

variable "game_sim_min_size" {
  description = "Minimum number of game simulation instances"
  type        = number
  default     = 1
}

variable "game_sim_max_size" {
  description = "Maximum number of game simulation instances"
  type        = number
  default     = 3
}

variable "game_sim_desired_capacity" {
  description = "Desired number of game simulation instances"
  type        = number
  default     = 1
}

variable "game_sim_udp_port" {
  description = "UDP port for game simulation service"
  type        = number
  default     = 7777
}

variable "target_autoscale_session_ratio" {
  description = "Target ratio of game sessions per instance for autoscaling"
  type        = number
  default     = 8
}

# DynamoDB Variables
variable "dynamodb_read_capacity" {
  description = "Number of read capacity units for DynamoDB tables"
  type        = number
  default     = 5
}

variable "dynamodb_write_capacity" {
  description = "Number of write capacity units for DynamoDB tables"
  type        = number
  default     = 5
}

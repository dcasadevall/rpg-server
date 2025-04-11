variable "vpc_id" {
  description = "The ID of the VPC where resources will be created"
  type        = string
}

variable "public_subnet_ids" {
  description = "List of public subnet IDs for EC2 instances"
  type        = list(string)
}

variable "alb_security_group_id" {
  description = "Security group ID of the ALB"
  type        = string
}

variable "alb_target_group_arn" {
  description = "The ARN of the ALB target group for the auto scaling group"
  type        = string
}

variable "ami_id" {
  description = "The AMI ID for the metadata service instances"
  type        = string
}

variable "instance_type" {
  description = "The EC2 instance type for the metadata service"
  type        = string
  default     = "t2.micro"
}

variable "min_size" {
  description = "Minimum number of instances in the ASG"
  type        = number
  default     = 1
}

variable "max_size" {
  description = "Maximum number of instances in the ASG"
  type        = number
  default     = 3
}

variable "desired_capacity" {
  description = "Desired number of instances in the ASG"
  type        = number
  default     = 1
}

variable "environment" {
  description = "Environment name (e.g., dev, prod)"
  type        = string
}

variable "dynamodb_instance_profile_arn" {
  description = "ARN of the IAM instance profile for DynamoDB access"
  type        = string
}

variable "metadata_repository_url" {
  description = "ECR repository URL for the metadata service"
  type        = string
}

variable "dynamodb_service_url" {
  description = "DynamoDB VPC endpoint URL"
  type        = string
}

variable "dynamodb_prefix_list_id" {
  description = "Prefix list ID for the DynamoDB VPC endpoint"
  type        = string
}

variable "user_data" {
  description = "User data script for the EC2 instances"
  type        = string
  default     = ""
}

variable "region" {
  description = "AWS region"
  type        = string
}
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
}

variable "max_size" {
  description = "Maximum number of instances in the ASG"
  type        = number
}

variable "desired_capacity" {
  description = "Desired number of instances in the ASG"
  type        = number
}

variable "gamesim_security_group_id" {
  description = "Security group ID of the game simulation service"
  type        = string
}

variable "dynamodb_instance_profile_arn" {
  description = "ARN of the DynamoDB instance profile"
  type        = string
}

variable "user_data" {
  description = "User data script for the EC2 instances"
  type        = string
}

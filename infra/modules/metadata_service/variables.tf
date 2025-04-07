variable "vpc_id" {
  description = "The ID of the VPC where the metadata service will be created"
  type        = string
}

variable "public_subnet_ids" {
  description = "List of public subnet IDs where the EC2 instances will be deployed"
  type        = list(string)
}

variable "alb_security_group_id" {
  description = "The ID of the ALB security group for ingress rules"
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
  description = "Minimum number of instances in the auto scaling group"
  type        = number
  default     = 1
}

variable "max_size" {
  description = "Maximum number of instances in the auto scaling group"
  type        = number
  default     = 3
}

variable "desired_capacity" {
  description = "Desired number of instances in the auto scaling group"
  type        = number
  default     = 2
}

variable "user_data" {
  description = "User data script for instance initialization"
  type        = string
  default     = ""
}

variable "gamesim_security_group_id" {
  description = "The ID of the Game Simulation security group for ingress rules"
  type        = string
  default     = ""
} 
variable "project_name" {
  description = "Name of the project used for resource naming and tagging"
  type        = string
}

variable "environment" {
  description = "Environment name (e.g., dev, prod)"
  type        = string
}

variable "region" {
  description = "AWS region"
  type        = string
}

variable "vpc_id" {
  description = "VPC ID where the ECS cluster will be deployed"
  type        = string
}

variable "public_subnet_ids" {
  description = "List of public subnet IDs for ECS tasks"
  type        = list(string)
}

variable "alb_security_group_id" {
  description = "Security group ID of the ALB"
  type        = string
}

variable "metadata_target_group_arn" {
  description = "ARN of the metadata service target group"
  type        = string
}

variable "metadata_repository_url" {
  description = "URL of the metadata service ECR repository"
  type        = string
}

variable "game_sim_repository_url" {
  description = "URL of the game simulation service ECR repository"
  type        = string
}

variable "game_sim_udp_port" {
  description = "UDP port for game simulation service"
  type        = number
}

variable "game_sim_desired_capacity" {
  description = "Desired number of game simulation service tasks"
  type        = number
}

variable "metadata_service_dns" {
  description = "DNS name of the metadata service"
  type        = string
}

variable "metadata_desired_capacity" {
  description = "Desired number of metadata service tasks"
  type        = number
  default     = 1
}

variable "cluster_name" {
  description = "Name of the ECS cluster"
  type        = string
}

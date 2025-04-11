variable "project_name" {
  description = "Name of the project used for resource naming and tagging"
  type        = string
}

variable "environment" {
  description = "Environment name (e.g., dev, prod)"
  type        = string
}

variable "vpc_id" {
  description = "VPC ID where the service will be deployed"
  type        = string
}

variable "public_subnet_ids" {
  description = "List of public subnet IDs for the service"
  type        = list(string)
}

variable "alb_security_group_id" {
  description = "Security group ID of the ALB"
  type        = string
}

variable "region" {
  description = "AWS region"
  type        = string
}

variable "ecs_cluster_id" {
  description = "ID of the ECS cluster"
  type        = string
}

variable "ecs_cluster_name" {
  description = "Name of the ECS cluster"
  type        = string
}

variable "ecs_task_execution_role_arn" {
  description = "ARN of the ECS task execution role"
  type        = string
}

variable "ecs_task_role_arn" {
  description = "ARN of the ECS task role"
  type        = string
}

variable "ecs_task_execution_role_policy_attachment" {
  description = "ECS task execution role policy attachment"
  type        = any
}

variable "metadata_repository_url" {
  description = "URL of the metadata service container repository"
  type        = string
}

variable "metadata_target_group_arn" {
  description = "ARN of the target group for the metadata service"
  type        = string
}

variable "metadata_desired_capacity" {
  description = "Desired number of metadata service tasks"
  type        = number
}

variable "metadata_min_size" {
  description = "Minimum number of metadata service tasks"
  type        = number
}

variable "metadata_max_size" {
  description = "Maximum number of metadata service tasks"
  type        = number
}

variable "target_autoscale_cpu_utilization" {
  description = "Target CPU utilization for autoscaling"
  type        = number
}

variable "target_autoscale_memory_utilization" {
  description = "Target memory utilization for autoscaling"
  type        = number
}

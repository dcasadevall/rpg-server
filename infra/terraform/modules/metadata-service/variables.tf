variable "vpc_id" {
  description = "ID of the VPC"
  type        = string
}

variable "public_subnet_ids" {
  description = "List of public subnet IDs"
  type        = list(string)
}

variable "metadata_repository_url" {
  description = "URL of the metadata service ECR repository"
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
  description = "Reference to the ECS task execution role policy attachment"
  type        = any
}

variable "alb_target_group_arn" {
  description = "ARN of the ALB target group for the metadata service"
  type        = string
}

variable "metadata_desired_capacity" {
  description = "Desired number of metadata service tasks"
  type        = number
}

variable "metadata_min_size" {
  description = "Minimum number of metadata service tasks"
  type        = number
  default     = 1
}

variable "metadata_max_size" {
  description = "Maximum number of metadata service tasks"
  type        = number
  default     = 3
}

variable "target_autoscale_cpu_utilization" {
  description = "Target CPU utilization percentage for autoscaling"
  type        = number
  default     = 70
}

variable "target_autoscale_memory_utilization" {
  description = "Target memory utilization percentage for autoscaling"
  type        = number
  default     = 80
}

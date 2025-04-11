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

variable "game_sim_repository_url" {
  description = "URL of the game simulation service container repository"
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

variable "game_sim_min_size" {
  description = "Minimum number of game simulation service tasks"
  type        = number
}

variable "game_sim_max_size" {
  description = "Maximum number of game simulation service tasks"
  type        = number
}

variable "target_autoscale_session_ratio" {
  description = "Target ratio of game sessions per instance for autoscaling"
  type        = number
}

variable "metadata_service_dns" {
  description = "DNS name of the metadata service"
  type        = string
}

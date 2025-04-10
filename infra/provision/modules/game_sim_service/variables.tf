variable "vpc_id" {
  description = "The ID of the VPC where the game simulation service will be created"
  type        = string
}

variable "public_subnet_ids" {
  description = "List of public subnet IDs where the EC2 instances will be deployed"
  type        = list(string)
}

variable "udp_port" {
  description = "UDP port for game simulation traffic"
  type        = number
  default     = 7777
}

variable "ami_id" {
  description = "The AMI ID for the game simulation service instances"
  type        = string
}

variable "instance_type" {
  description = "The EC2 instance type for the game simulation service"
  type        = string
  default     = "c5.large"  # Game simulations often need more compute power
}

variable "min_size" {
  description = "Minimum number of instances in the auto scaling group"
  type        = number
  default     = 1
}

variable "max_size" {
  description = "Maximum number of instances in the auto scaling group"
  type        = number
  default     = 10  # Game sims may need to scale more
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

variable "target_autoscale_session_ratio" {
  description = "Average number of active game sessions to target for autoscaling. 8 = 80% session load."
  type        = number
  default     = 8
}
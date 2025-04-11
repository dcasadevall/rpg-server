variable "vpc_id" {
  description = "The ID of the VPC where the ALB will be created"
  type        = string
}

variable "public_subnet_ids" {
  description = "List of public subnet IDs where the ALB will be deployed"
  type        = list(string)
}

variable "domain_name" {
  description = "Domain name for the ACM certificate (e.g., example.com)"
  type        = string
}

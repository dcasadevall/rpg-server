variable "environment" {
  description = "Environment name (e.g., dev, prod)"
  type        = string
}

variable "items_table_name" {
  description = "Name of the items table to seed"
  type        = string
}

variable "region" {
  description = "AWS region"
  type        = string
}

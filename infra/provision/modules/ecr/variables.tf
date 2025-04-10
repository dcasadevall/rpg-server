variable "project_name" {
  description = "Name of the project used for resource naming and tagging"
  type        = string
}

variable "tags" {
  description = "Tags to apply to all resources"
  type        = map(string)
  default     = {}
}

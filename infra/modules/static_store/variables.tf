variable "bucket_name" {
  description = "Name of the S3 bucket for static content"
  type        = string
}

variable "region" {
  description = "AWS region to deploy the static store in"
  type        = string
  default     = "us-east-1"
}

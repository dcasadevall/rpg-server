variable "characters_table_name" {
  description = "Name of the DynamoDB table for characters"
  type        = string
  default     = "characters"
}

variable "items_table_name" {
  description = "Name of the DynamoDB table for items"
  type        = string
  default     = "items"
}

variable "billing_mode" {
  description = "DynamoDB billing mode (PROVISIONED or PAY_PER_REQUEST)"
  type        = string
  default     = "PAY_PER_REQUEST"

  validation {
    condition     = contains(["PROVISIONED", "PAY_PER_REQUEST"], var.billing_mode)
    error_message = "Billing mode must be either PROVISIONED or PAY_PER_REQUEST."
  }
}

variable "read_capacity" {
  description = "Number of read capacity units for the table (only used if billing_mode is PROVISIONED)"
  type        = number
  default     = 5
}

variable "write_capacity" {
  description = "Number of write capacity units for the table (only used if billing_mode is PROVISIONED)"
  type        = number
  default     = 5
}

variable "tags" {
  description = "A map of tags to add to all resources"
  type        = map(string)
  default     = {}
}

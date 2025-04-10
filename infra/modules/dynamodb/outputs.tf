output "instance_profile_arn" {
  description = "ARN of the IAM instance profile for DynamoDB access"
  value       = aws_iam_instance_profile.ec2_dynamodb_profile.arn
}

output "iam_role_arn" {
  description = "ARN of the IAM role for DynamoDB access"
  value       = aws_iam_role.ec2_dynamodb_role.arn
}

output "iam_policy_arn" {
  description = "ARN of the IAM policy for DynamoDB access"
  value       = aws_iam_policy.dynamodb_access.arn
}

output "characters_table_arn" {
  description = "ARN of the characters table"
  value       = aws_dynamodb_table.characters.arn
}

output "items_table_arn" {
  description = "ARN of the items table"
  value       = aws_dynamodb_table.items.arn
}

output "characters_table_name" {
  description = "Name of the characters table"
  value       = aws_dynamodb_table.characters.name
}

output "items_table_name" {
  description = "Name of the items table"
  value       = aws_dynamodb_table.items.name
}

output "characters_table_stream_arn" {
  description = "ARN of the characters table stream (if enabled)"
  value       = aws_dynamodb_table.characters.stream_arn
}

output "items_table_stream_arn" {
  description = "ARN of the items table stream (if enabled)"
  value       = aws_dynamodb_table.items.stream_arn
}

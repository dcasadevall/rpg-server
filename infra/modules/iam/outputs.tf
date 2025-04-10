output "policy_arn" {
  description = "ARN of the infrastructure management policy"
  value       = aws_iam_policy.infrastructure_management.arn
}

output "policy_name" {
  description = "Name of the infrastructure management policy"
  value       = aws_iam_policy.infrastructure_management.name
}

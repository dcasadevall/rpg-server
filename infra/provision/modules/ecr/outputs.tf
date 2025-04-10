output "repository_url" {
  description = "The URL of the ECR repository"
  value       = aws_ecr_repository.character_service.repository_url
}

output "repository_arn" {
  description = "The ARN of the ECR repository"
  value       = aws_ecr_repository.character_service.arn
}

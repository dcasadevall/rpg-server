output "repository_arn" {
  description = "The ARN of the metadata ECR repository"
  value       = aws_ecr_repository.metadata_service.arn
}

output "metadata_repository_url" {
  description = "The URL of the metadata service ECR repository"
  value       = aws_ecr_repository.metadata_service.repository_url
}

output "metadata_repository_arn" {
  description = "The ARN of the metadata service ECR repository"
  value       = aws_ecr_repository.metadata_service.arn
}

output "game_sim_repository_url" {
  description = "The URL of the game simulation service ECR repository"
  value       = aws_ecr_repository.game_sim_service.repository_url
}

output "game_sim_repository_arn" {
  description = "The ARN of the game simulation service ECR repository"
  value       = aws_ecr_repository.game_sim_service.arn
}

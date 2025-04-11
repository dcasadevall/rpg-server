output "cluster_id" {
  description = "ID of the ECS cluster"
  value       = aws_ecs_cluster.cluster.id
}

output "cluster_arn" {
  description = "ARN of the ECS cluster"
  value       = aws_ecs_cluster.cluster.arn
}

output "cluster_name" {
  description = "Name of the ECS cluster"
  value       = aws_ecs_cluster.cluster.name
}

output "ecs_task_execution_role_arn" {
  description = "ARN of the ECS task execution role"
  value       = aws_iam_role.ecs_task_execution_role.arn
}

output "ecs_task_role_arn" {
  description = "ARN of the ECS task role"
  value       = aws_iam_role.ecs_task_role.arn
}

output "ecs_task_role_id" {
  description = "ID of the ECS task role"
  value       = aws_iam_role.ecs_task_role.id
}

output "ecs_task_execution_role_policy_attachment" {
  description = "ECS task execution role policy attachment"
  value       = aws_iam_role_policy_attachment.ecs_task_execution_role
}

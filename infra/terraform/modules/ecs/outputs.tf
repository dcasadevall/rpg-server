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

output "metadata_service_security_group_id" {
  description = "ID of the metadata service security group"
  value       = aws_security_group.metadata.id
}

output "game_sim_service_security_group_id" {
  description = "ID of the game simulation service security group"
  value       = aws_security_group.game_sim.id
}

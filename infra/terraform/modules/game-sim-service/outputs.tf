output "game_sim_service_security_group_id" {
  description = "ID of the game simulation service security group"
  value       = aws_security_group.game_sim.id
}

output "game_sim_task_definition_arn" {
  description = "ARN of the game simulation service task definition"
  value       = aws_ecs_task_definition.game_sim.arn
}

output "game_sim_service_name" {
  description = "Name of the game simulation service"
  value       = aws_ecs_service.game_sim.name
}

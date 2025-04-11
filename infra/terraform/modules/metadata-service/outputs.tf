output "metadata_service_sg_id" {
  description = "ID of the metadata service security group"
  value       = aws_security_group.metadata.id
}

output "metadata_service_name" {
  description = "Name of the metadata service"
  value       = aws_ecs_service.metadata.name
}

output "metadata_task_definition_arn" {
  description = "ARN of the metadata service task definition"
  value       = aws_ecs_task_definition.metadata.arn
}

output "metadata_service_arn" {
  description = "ARN of the metadata service"
  value       = aws_ecs_service.metadata.id
}

output "metadata_log_group_name" {
  description = "Name of the CloudWatch log group for metadata service"
  value       = aws_cloudwatch_log_group.metadata.name
}

output "metadata_security_group_id" {
  description = "ID of the metadata service security group"
  value       = aws_security_group.metadata.id
}

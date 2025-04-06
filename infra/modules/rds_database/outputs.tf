output "rds_endpoint" {
  description = "The connection endpoint for the RDS instance"
  value       = aws_db_instance.rds.endpoint
}

output "rds_security_group_id" {
  description = "The ID of the security group for the RDS instance"
  value       = aws_security_group.rds_sg.id
}

output "rds_name" {
  description = "The name of the database"
  value       = aws_db_instance.rds.name
}

output "rds_username" {
  description = "The master username for the database"
  value       = aws_db_instance.rds.username
  sensitive   = true
} 
output "metadata_service_asg_name" {
  description = "The name of the auto scaling group for the metadata service"
  value       = aws_autoscaling_group.metadata_asg.name
}

output "metadata_service_security_group_id" {
  description = "The ID of the security group for the metadata service"
  value       = aws_security_group.metadata_sg.id
}

output "metadata_service_launch_template_id" {
  description = "The ID of the launch template for the metadata service"
  value       = aws_launch_template.metadata_lt.id
}

output "metadata_service_dns" {
  description = "The DNS name of the metadata service"
  value       = aws_launch_template.metadata_lt.name
}

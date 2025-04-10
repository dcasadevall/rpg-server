output "gamesim_service_asg_name" {
  description = "The name of the auto scaling group for the game simulation service"
  value       = aws_autoscaling_group.gamesim_asg.name
}

output "gamesim_security_group_id" {
  description = "The ID of the security group for the game simulation service"
  value       = aws_security_group.gamesim_sg.id
}

output "gamesim_launch_template_id" {
  description = "The ID of the launch template for the game simulation service"
  value       = aws_launch_template.gamesim_lt.id
}

output "udp_port" {
  description = "The UDP port used for game session traffic"
  value       = var.udp_port
} 
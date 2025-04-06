output "alb_dns_name" {
  description = "The DNS name of the Application Load Balancer"
  value       = aws_lb.alb.dns_name
}

output "alb_target_group_arn" {
  description = "The ARN of the ALB target group"
  value       = aws_lb_target_group.metadata_tg.arn
}

output "alb_security_group_id" {
  description = "The ID of the security group attached to the ALB"
  value       = aws_security_group.alb_sg.id
} 
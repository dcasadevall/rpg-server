output "alb_target_group_arn" {
  description = "ARN of the ALB target group"
  value       = aws_lb_target_group.metadata_tg.arn
}

output "alb_security_group_id" {
  description = "ID of the ALB security group"
  value       = aws_security_group.alb_sg.id
}

output "route53_zone_id" {
  description = "ID of the Route 53 hosted zone"
  value       = aws_route53_zone.main.zone_id
}

output "alb_dns_name" {
  description = "DNS name of the ALB"
  value       = aws_lb.alb.dns_name
}

output "api_domain_name" {
  description = "Domain name for the API endpoint"
  value       = aws_route53_record.alb.fqdn
}

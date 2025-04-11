# VPC and Networking Outputs
output "vpc_id" {
  description = "ID of the VPC"
  value       = module.vpc.vpc_id
}

output "public_subnet_ids" {
  description = "IDs of the public subnets"
  value       = module.vpc.public_subnet_ids
}

# ALB Outputs
output "alb_dns_name" {
  description = "DNS name of the ALB"
  value       = module.alb.alb_dns_name
}

output "api_domain_name" {
  description = "Domain name for the API endpoint"
  value       = module.alb.api_domain_name
}

# DynamoDB Outputs
output "characters_table_name" {
  description = "Name of the characters DynamoDB table"
  value       = module.dynamodb.characters_table_name
}

output "items_table_name" {
  description = "Name of the items DynamoDB table"
  value       = module.dynamodb.items_table_name
}

# Static Store Outputs
output "s3_bucket_name" {
  description = "Name of the S3 bucket for static content"
  value       = module.static_store.s3_bucket_name
}

output "cloudfront_domain_name" {
  description = "Domain name of the CloudFront distribution"
  value       = module.static_store.cloudfront_domain_name
}

output "metadata_repository_url" {
  description = "The URL of the metadata service ECR repository"
  value       = module.ecr.metadata_repository_url
}

output "game_sim_repository_url" {
  description = "The URL of the game simulation service ECR repository"
  value       = module.ecr.game_sim_repository_url
}

output "region" {
  description = "The AWS region"
  value       = var.region
}

output "vpc_id" {
  description = "ID of the VPC"
  value       = aws_vpc.main.id
}

output "public_subnet_ids" {
  description = "List of public subnet IDs"
  value       = aws_subnet.public[*].id
}

output "public_route_table_id" {
  description = "ID of the public route table"
  value       = aws_route_table.public.id
}

output "internet_gateway_id" {
  description = "ID of the Internet Gateway"
  value       = aws_internet_gateway.gw.id
}

output "dynamodb_prefix_list_id" {
  description = "Prefix list ID for the DynamoDB VPC endpoint"
  value       = aws_vpc_endpoint.dynamodb.prefix_list_id
}

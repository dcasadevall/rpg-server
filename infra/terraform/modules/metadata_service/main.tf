# Metadata Service Module

# Create Security Group for Metadata Service
resource "aws_security_group" "metadata_sg" {
  name        = "metadata-service-sg"
  description = "Security group for metadata service"
  vpc_id      = var.vpc_id
}

# Allow HTTPS traffic from the ALB
resource "aws_security_group_rule" "alb_ingress" {
  type              = "ingress"
  from_port         = 443
  to_port           = 443
  protocol          = "tcp"
  security_group_id = aws_security_group.metadata_sg.id
  source_security_group_id = var.alb_security_group_id
  description       = "Allow HTTPS traffic from ALB"
}

# Allow responses to the ALB
resource "aws_security_group_rule" "alb_egress" {
  type              = "egress"
  from_port         = 443
  to_port           = 443
  protocol          = "tcp"
  security_group_id = aws_security_group.metadata_sg.id
  source_security_group_id = var.alb_security_group_id
  description       = "Allow responses to ALB"
}

# Allow outbound traffic to DynamoDB VPC endpoint
resource "aws_security_group_rule" "dynamodb_egress" {
  type              = "egress"
  from_port         = 443
  to_port           = 443
  protocol          = "tcp"
  security_group_id = aws_security_group.metadata_sg.id
  prefix_list_ids   = [var.dynamodb_prefix_list_id]
  description       = "Allow outbound traffic to DynamoDB"
}

# Allow HTTP traffic from other instances in the VPC (Meant for GameSim updates)
resource "aws_security_group_rule" "vpc_ingress" {
  type              = "ingress"
  from_port         = 80
  to_port           = 80
  protocol          = "tcp"
  security_group_id = aws_security_group.metadata_sg.id
  self              = true
  description       = "Allow HTTP traffic from other instances in the VPC"
}

# Launch Template
resource "aws_launch_template" "metadata_lt" {
  name_prefix   = "metadata-service-"
  image_id      = var.ami_id
  instance_type = var.instance_type

  # Network configuration for EC2 instances
  network_interfaces {
    associate_public_ip_address = true
    security_groups             = [aws_security_group.metadata_sg.id]
  }

  # IAM instance profile for DynamoDB access
  iam_instance_profile {
    arn = var.dynamodb_instance_profile_arn
  }

  # User data script that installs Docker and runs our container
  user_data = base64encode(<<-EOF
    #!/bin/bash
    # Install Docker
    yum update -y
    amazon-linux-extras install docker
    service docker start
    usermod -a -G docker ec2-user

    # Login to ECR
    aws ecr get-login-password --region ${data.aws_region.current.name} | docker login --username AWS --password-stdin ${replace(var.metadata_repository_url, "/^([^/]+).*$/", "$1")}

    # Pull and run the metadata service container
    docker pull ${var.metadata_repository_url}:latest
    docker run -d \
      -e ENVIRONMENT=${var.environment} \
      -e DYNAMODB_DB_PREFIX=${var.environment}- \
      -e DYNAMODB_SERVICE_URL=dynamodb.${data.aws_region.current.name}.amazonaws.com \
      -p 80:80 \
      ${var.metadata_repository_url}:latest
  EOF
  )
}

# Auto Scaling Group
resource "aws_autoscaling_group" "metadata_asg" {
  name                      = "metadata-service-asg"
  min_size                  = var.min_size
  max_size                  = var.max_size
  desired_capacity          = var.desired_capacity
  vpc_zone_identifier       = var.public_subnet_ids
  launch_template {
    id      = aws_launch_template.metadata_lt.id
    version = "$Latest"
  }
  health_check_type         = "EC2"
  health_check_grace_period = 60
  force_delete              = true
}

data "aws_region" "current" {}

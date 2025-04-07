# Metadata Service Module

# Create Security Group for Metadata Service
resource "aws_security_group" "metadata_sg" {
  name        = "metadata-service-sg"
  description = "Allow HTTPS traffic from ALB and Game Sim Service"
  vpc_id      = var.vpc_id

  # Allow HTTPS traffic from the ALB
  ingress {
    from_port       = 443
    to_port         = 443
    protocol        = "tcp"
    security_groups = [var.alb_security_group_id]
  }

  # Allow HTTPS traffic from Game Sim service for session updates and heartbeats
  ingress {
    from_port       = 443
    to_port         = 443
    protocol        = "tcp"
    security_groups = [var.gamesim_security_group_id]
  }

  # Allow all outbound traffic from Metadata service instances
  # This allows the service to communicate with other AWS services and the internet
  egress {
    from_port   = 0
    to_port     = 0
    protocol    = "-1"
    cidr_blocks = ["0.0.0.0/0"]
  }
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

  # User data script that runs on instance startup (base64 encoded)
  # This typically installs and configures the metadata service software
  user_data = base64encode(var.user_data)
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
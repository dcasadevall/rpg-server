# Game Simulation Service Module

# Create Security Group for GameSim Instances
resource "aws_security_group" "gamesim_sg" {
  name        = "game-sim-service-sg"
  description = "Security group for game simulation service"
  vpc_id      = var.vpc_id
}

# Allow inbound UDP traffic for game simulation
resource "aws_security_group_rule" "udp_ingress" {
  type              = "ingress"
  from_port         = var.udp_port
  to_port           = var.udp_port
  protocol          = "udp"
  security_group_id = aws_security_group.gamesim_sg.id
  cidr_blocks       = ["0.0.0.0/0"]
  description       = "Allow UDP traffic for game simulation"
}

# Allow outbound UDP traffic to public
resource "aws_security_group_rule" "udp_egress" {
  type              = "egress"
  from_port         = var.udp_port
  to_port           = var.udp_port
  protocol          = "udp"
  security_group_id = aws_security_group.gamesim_sg.id
  cidr_blocks       = ["0.0.0.0/0"]
  description       = "Allow outbound UDP traffic to public"
}

# Allow HTTP traffic to other instances in the VPC (Meant for Metadata Service)
resource "aws_security_group_rule" "vpc_egress" {
  type              = "egress"
  from_port         = 80
  to_port           = 80
  protocol          = "tcp"
  security_group_id = aws_security_group.gamesim_sg.id
  self              = true
  description       = "Allow HTTP traffic to other instances in the VPC"
}

# Launch Template
resource "aws_launch_template" "gamesim_lt" {
  name_prefix   = "game-sim-service-"
  image_id      = var.ami_id
  instance_type = var.instance_type

  # Network configuration for EC2 instances
  network_interfaces {
    associate_public_ip_address = true
    security_groups             = [aws_security_group.gamesim_sg.id]
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
    aws ecr get-login-password --region ${data.aws_region.current.name} | docker login --username AWS --password-stdin ${replace(var.game_sim_repository_url, "/^([^/]+).*$/", "$1")}

    # Pull and run the game simulation service container
    docker pull ${var.game_sim_repository_url}:latest
    docker run -d \
      -e GAME_SIM_PORT=${var.udp_port} \
      -e GAME_SIM_ENVIRONMENT=${var.environment} \
      -e METADATA_SERVICE_URL=https://${var.metadata_service_dns} \
      -p ${var.udp_port}:${var.udp_port}/udp \
      ${var.game_sim_repository_url}:latest
  EOF
  )
}

data "aws_region" "current" {}

# Auto Scaling Group
resource "aws_autoscaling_group" "gamesim_asg" {
  name                      = "game-sim-service-asg"
  min_size                  = var.min_size
  max_size                  = var.max_size
  desired_capacity          = var.desired_capacity
  vpc_zone_identifier       = var.public_subnet_ids
  launch_template {
    id      = aws_launch_template.gamesim_lt.id
    version = "$Latest"
  }
  health_check_type         = "EC2"
  health_check_grace_period = 60
  force_delete              = true
}

# CloudWatch Metric Alarm for scaling up
resource "aws_cloudwatch_metric_alarm" "scale_up" {
  alarm_name          = "game-sim-scale-up"
  comparison_operator = "GreaterThanThreshold"
  evaluation_periods  = "2"
  metric_name         = "ActiveGameSessions"
  namespace           = "GameSimService"
  period             = "60"
  statistic          = "Sum"
  threshold          = var.target_autoscale_session_ratio * var.desired_capacity
  alarm_description  = "Scale up when active game sessions exceed target ratio"
  alarm_actions      = [aws_autoscaling_policy.scale_up.arn]

  dimensions = {
    AutoScalingGroupName = aws_autoscaling_group.gamesim_asg.name
  }
}

# CloudWatch Metric Alarm for scaling down
resource "aws_cloudwatch_metric_alarm" "scale_down" {
  alarm_name          = "game-sim-scale-down"
  comparison_operator = "LessThanThreshold"
  evaluation_periods  = "2"
  metric_name         = "ActiveGameSessions"
  namespace           = "GameSimService"
  period             = "60"
  statistic          = "Sum"
  threshold          = var.target_autoscale_session_ratio * (var.desired_capacity - 1)
  alarm_description  = "Scale down when active game sessions fall below target ratio"
  alarm_actions      = [aws_autoscaling_policy.scale_down.arn]

  dimensions = {
    AutoScalingGroupName = aws_autoscaling_group.gamesim_asg.name
  }
}

# Scaling Up Policy
resource "aws_autoscaling_policy" "scale_up" {
  name                   = "game-sim-scale-up"
  scaling_adjustment     = 1
  adjustment_type        = "ChangeInCapacity"
  cooldown              = 300
  autoscaling_group_name = aws_autoscaling_group.gamesim_asg.name
}

# Scaling Down Policy
resource "aws_autoscaling_policy" "scale_down" {
  name                   = "game-sim-scale-down"
  scaling_adjustment     = -1
  adjustment_type        = "ChangeInCapacity"
  cooldown              = 300
  autoscaling_group_name = aws_autoscaling_group.gamesim_asg.name
}

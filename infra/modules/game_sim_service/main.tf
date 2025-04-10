# Game Simulation Service Module

# Create Security Group for Game Sim Instances
resource "aws_security_group" "gamesim_sg" {
  name        = "game-sim-service-sg"
  description = "Allow UDP traffic for game sessions"
  vpc_id      = var.vpc_id

  ingress {
    from_port   = var.udp_port
    to_port     = var.udp_port
    protocol    = "udp"
    cidr_blocks = ["0.0.0.0/0"]
  }

  egress {
    from_port   = 0
    to_port     = 0
    protocol    = "-1"
    cidr_blocks = ["0.0.0.0/0"]
  }
}

# Launch Template
resource "aws_launch_template" "gamesim_lt" {
  name_prefix   = "game-sim-service-"
  image_id      = var.ami_id
  instance_type = var.instance_type

  network_interfaces {
    associate_public_ip_address = true
    security_groups             = [aws_security_group.gamesim_sg.id]
  }

  user_data = base64encode(var.user_data)
}

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

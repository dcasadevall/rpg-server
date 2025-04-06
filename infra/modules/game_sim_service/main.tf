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

# Autoscaling policy that allows us to watch
# for a certain number of active game sessions
resource "aws_autoscaling_policy" "gamesim_target_tracking" {
  name                   = "gamesim-target-tracking"
  autoscaling_group_name = aws_autoscaling_group.gamesim_asg.name
  policy_type            = "TargetTrackingScaling"

  target_tracking_configuration {
    customized_metric_specification {
      metric_math_specification {
        expression = "SUM(m1) / COUNT(m1)"
        label      = "SessionsPerInstance"

        metric {
          id         = "m1"
          metric_name = "ActiveGameSessions"
          namespace   = "GameSimService"
          stat        = "Sum"
          period      = 60
        }
      }

      target_value = var.target_autoscale_session_ratio
    }

    predefined_metric_specification {
      predefined_metric_type = "ASGAverageCPUUtilization"
    }

    scale_in_cooldown  = 300
    scale_out_cooldown = 300
  }
} 

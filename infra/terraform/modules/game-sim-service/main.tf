# Game Simulation Service Module

# Create Security Group
resource "aws_security_group" "game_sim" {
  name        = "game-sim-service-sg"
  description = "Security group for game simulation service"
  vpc_id      = var.vpc_id

  ingress {
    from_port   = var.game_sim_udp_port
    to_port     = var.game_sim_udp_port
    protocol    = "udp"
    cidr_blocks = ["0.0.0.0/0"]
    description = "Allow UDP traffic for game simulation"
  }

  # Allow outbound traffic to metadata service
  egress {
    from_port       = 443
    to_port         = 443
    protocol        = "tcp"
    security_groups = [var.metadata_service_security_group_id]
    description     = "Allow HTTPS traffic to metadata service"
  }

  egress {
    from_port   = 0
    to_port     = 0
    protocol    = "-1"
    cidr_blocks = ["0.0.0.0/0"]
  }
}

# Create ECS Task Definition
resource "aws_ecs_task_definition" "game_sim" {
  family                   = "game-sim-service"
  network_mode             = "awsvpc"
  requires_compatibilities = ["FARGATE"]
  cpu                      = 1024
  memory                   = 2048
  execution_role_arn       = var.ecs_task_execution_role_arn
  task_role_arn            = var.ecs_task_role_arn

  container_definitions = jsonencode([
    {
      name      = "game-sim-service"
      image     = var.game_sim_repository_url
      essential = true
      portMappings = [
        {
          containerPort = var.game_sim_udp_port
          hostPort      = var.game_sim_udp_port
          protocol      = "udp"
        }
      ]
      environment = [
        {
          name  = "GAME_SIM_PORT"
          value = tostring(var.game_sim_udp_port)
        },
        {
          name  = "GAME_SIM_ENVIRONMENT"
          value = var.environment
        },
        {
          name  = "METADATA_SERVICE_URL"
          value = "https://${var.metadata_service_dns}"
        }
      ]
      logConfiguration = {
        logDriver = "awslogs"
        options = {
          awslogs-group         = aws_cloudwatch_log_group.game_sim.name
          awslogs-region        = var.region
          awslogs-stream-prefix = "ecs"
        }
      }
    }
  ])
}

# Create ECS Service
resource "aws_ecs_service" "game_sim" {
  name            = "game-sim-service"
  cluster         = var.ecs_cluster_id
  task_definition = aws_ecs_task_definition.game_sim.arn
  desired_count   = var.game_sim_desired_capacity
  launch_type     = "FARGATE"

  network_configuration {
    subnets          = var.public_subnet_ids
    security_groups  = [aws_security_group.game_sim.id]
    assign_public_ip = true
  }

  depends_on = [var.ecs_task_execution_role_policy_attachment]
}

# CloudWatch Log Group
resource "aws_cloudwatch_log_group" "game_sim" {
  name              = "/ecs/game-sim-service"
  retention_in_days = 30
}

# Create Application Auto Scaling Target
resource "aws_appautoscaling_target" "game_sim" {
  max_capacity       = var.game_sim_max_size
  min_capacity       = var.game_sim_min_size
  resource_id        = "service/${var.ecs_cluster_name}/${aws_ecs_service.game_sim.name}"
  scalable_dimension = "ecs:service:DesiredCount"
  service_namespace  = "ecs"
}

# Create CloudWatch Metric Alarm for Sessions Per Instance
resource "aws_cloudwatch_metric_alarm" "sessions_per_instance_alarm" {
  alarm_name          = "sessions-per-instance-too-high"
  comparison_operator = "GreaterThanThreshold"
  evaluation_periods  = 2
  threshold           = var.target_autoscale_session_ratio
  alarm_description   = "Alarm when SessionsPerInstance exceeds target ratio"
  treat_missing_data  = "breaching"
  alarm_actions       = [aws_appautoscaling_policy.game_sim_scale_up.arn]

  metric_query {
    id          = "m1"
    metric {
      namespace   = "GameSimulation"
      metric_name = "ActiveGameSessions"
      period      = 60
      stat        = "Average"
      dimensions = {
        ServiceName = aws_ecs_service.game_sim.name
        ClusterName = var.ecs_cluster_name
      }
    }
    return_data = false
  }

  metric_query {
    id          = "m2"
    metric {
      namespace   = "AWS/AutoScaling"
      metric_name = "GroupInServiceInstances"
      dimensions = {
        AutoScalingGroupName = aws_ecs_service.game_sim.name
      }
      period = 60
      stat   = "Average"
    }
    return_data = false
  }

  metric_query {
    id          = "e1"
    expression  = "IF(m2 > 0, m1 / m2, 0)"
    label       = "SessionsPerInstance"
    return_data = true
  }
}

# Create Scaling Up Policy
resource "aws_appautoscaling_policy" "game_sim_scale_up" {
  name               = "game-sim-scale-up"
  policy_type        = "StepScaling"
  resource_id        = aws_appautoscaling_target.game_sim.resource_id
  scalable_dimension = aws_appautoscaling_target.game_sim.scalable_dimension
  service_namespace  = aws_appautoscaling_target.game_sim.service_namespace

  step_scaling_policy_configuration {
    adjustment_type         = "ChangeInCapacity"
    cooldown               = 300
    metric_aggregation_type = "Average"

    step_adjustment {
      scaling_adjustment          = 1
      metric_interval_lower_bound = 0
    }
  }
}

# Create Scaling Down Policy
resource "aws_appautoscaling_policy" "game_sim_scale_down" {
  name               = "game-sim-scale-down"
  policy_type        = "StepScaling"
  resource_id        = aws_appautoscaling_target.game_sim.resource_id
  scalable_dimension = aws_appautoscaling_target.game_sim.scalable_dimension
  service_namespace  = aws_appautoscaling_target.game_sim.service_namespace

  step_scaling_policy_configuration {
    adjustment_type         = "ChangeInCapacity"
    cooldown               = 300
    metric_aggregation_type = "Average"

    step_adjustment {
      scaling_adjustment          = -1
      metric_interval_upper_bound = 0
    }
  }
}

# Create CloudWatch Metric Alarm for Scaling Down
resource "aws_cloudwatch_metric_alarm" "sessions_per_instance_low_alarm" {
  alarm_name          = "sessions-per-instance-too-low"
  comparison_operator = "LessThanThreshold"
  evaluation_periods  = 2
  threshold           = var.target_autoscale_session_ratio * 0.5  # Scale down when sessions are 50% below target
  alarm_description   = "Alarm when SessionsPerInstance is below target ratio"
  treat_missing_data  = "breaching"
  alarm_actions       = [aws_appautoscaling_policy.game_sim_scale_down.arn]

  metric_query {
    id          = "m1"
    metric {
      namespace   = "GameSimulation"
      metric_name = "ActiveGameSessions"
      period      = 60
      stat        = "Average"
      dimensions = {
        ServiceName = aws_ecs_service.game_sim.name
        ClusterName = var.ecs_cluster_name
      }
    }
    return_data = false
  }

  metric_query {
    id          = "m2"
    metric {
      namespace   = "AWS/AutoScaling"
      metric_name = "GroupInServiceInstances"
      dimensions = {
        AutoScalingGroupName = aws_ecs_service.game_sim.name
      }
      period = 60
      stat   = "Average"
    }
    return_data = false
  }

  metric_query {
    id          = "e1"
    expression  = "IF(m2 > 0, m1 / m2, 0)"
    label       = "SessionsPerInstance"
    return_data = true
  }
}
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

# Create Target Tracking Scaling Policy
resource "aws_appautoscaling_policy" "game_sim_target_tracking" {
  name               = "game-sim-target-tracking"
  policy_type        = "TargetTrackingScaling"
  resource_id        = aws_appautoscaling_target.game_sim.resource_id
  scalable_dimension = aws_appautoscaling_target.game_sim.scalable_dimension
  service_namespace  = aws_appautoscaling_target.game_sim.service_namespace

  target_tracking_scaling_policy_configuration {
    target_value = var.target_autoscale_session_ratio

    customized_metric_specification {
      metric_math_specification {
        expression = "SUM(m1) / COUNT(m1)"
        label      = "SessionsPerInstance"

        metric {
          id         = "m1"
          metric_name = "ActiveGameSessions"
          namespace   = "GameSimulation"
          stat        = "Sum"
          period      = 60

          dimensions {
            name  = "ServiceName"
            value = aws_ecs_service.game_sim.name
          }
          dimensions {
            name  = "ClusterName"
            value = var.ecs_cluster_name
          }
        }
      }
    }

    scale_in_cooldown  = 300
    scale_out_cooldown = 300
  }
}

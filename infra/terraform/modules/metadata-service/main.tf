# Metadata Service Module

# Create Security Group
resource "aws_security_group" "metadata" {
  name        = "metadata-service-sg"
  description = "Security group for metadata service"
  vpc_id      = var.vpc_id

  ingress {
    from_port       = 80
    to_port         = 80
    protocol        = "tcp"
    security_groups = [var.alb_security_group_id]
    description     = "Allow HTTP traffic from ALB"
  }

  egress {
    from_port   = 0
    to_port     = 0
    protocol    = "-1"
    cidr_blocks = ["0.0.0.0/0"]
  }
}

# Create ECS Task Definition
resource "aws_ecs_task_definition" "metadata" {
  family                   = "metadata-service"
  network_mode             = "awsvpc"
  requires_compatibilities = ["FARGATE"]
  cpu                      = 256
  memory                   = 512
  execution_role_arn       = var.ecs_task_execution_role_arn
  task_role_arn            = var.ecs_task_role_arn

  runtime_platform {
    operating_system_family = "LINUX"
    cpu_architecture        = "X86_64"
  }

  container_definitions = jsonencode([
    {
      name      = "metadata-service"
      image     = var.metadata_repository_url
      essential = true
      portMappings = [
        {
          containerPort = 80
          hostPort      = 80
          protocol      = "tcp"
        }
      ]
      environment = [
        {
          name  = "ENVIRONMENT"
          value = var.environment
        },
        {
          name  = "DYNAMODB_DB_PREFIX"
          value = "${var.environment}-"
        },
        {
          name  = "DYNAMODB_SERVICE_URL"
          value = "https://dynamodb.${var.region}.amazonaws.com"
        },
        {
          name  = "DB_TYPE"
          value = "dynamo-db"
        }
      ]
      logConfiguration = {
        logDriver = "awslogs"
        options = {
          awslogs-group         = aws_cloudwatch_log_group.metadata.name
          awslogs-region        = var.region
          awslogs-stream-prefix = "ecs"
        }
      }
    }
  ])
}

# Create ECS Service
resource "aws_ecs_service" "metadata" {
  name            = "metadata-service"
  cluster         = var.ecs_cluster_id
  task_definition = aws_ecs_task_definition.metadata.arn
  desired_count   = var.metadata_desired_capacity
  launch_type     = "FARGATE"

  network_configuration {
    subnets          = var.public_subnet_ids
    security_groups  = [aws_security_group.metadata.id]
    assign_public_ip = true
  }

  load_balancer {
    target_group_arn = var.metadata_target_group_arn
    container_name   = "metadata-service"
    container_port   = 80
  }

  depends_on = [var.ecs_task_execution_role_policy_attachment]
}

# CloudWatch Log Group
resource "aws_cloudwatch_log_group" "metadata" {
  name              = "/ecs/metadata-service"
  retention_in_days = 30
}

# Create Application Auto Scaling Target
resource "aws_appautoscaling_target" "metadata" {
  max_capacity       = var.metadata_max_size
  min_capacity       = var.metadata_min_size
  resource_id        = "service/${var.ecs_cluster_name}/${aws_ecs_service.metadata.name}"
  scalable_dimension = "ecs:service:DesiredCount"
  service_namespace  = "ecs"
}

# Create CPU Target Tracking Scaling Policy
resource "aws_appautoscaling_policy" "metadata_cpu_tracking" {
  name               = "metadata-cpu-tracking"
  policy_type        = "TargetTrackingScaling"
  resource_id        = aws_appautoscaling_target.metadata.resource_id
  scalable_dimension = aws_appautoscaling_target.metadata.scalable_dimension
  service_namespace  = aws_appautoscaling_target.metadata.service_namespace

  target_tracking_scaling_policy_configuration {
    target_value = var.target_autoscale_cpu_utilization

    predefined_metric_specification {
      predefined_metric_type = "ECSServiceAverageCPUUtilization"
    }

    scale_in_cooldown  = 300
    scale_out_cooldown = 300
  }
}

# Create Memory Target Tracking Scaling Policy
resource "aws_appautoscaling_policy" "metadata_memory_tracking" {
  name               = "metadata-memory-tracking"
  policy_type        = "TargetTrackingScaling"
  resource_id        = aws_appautoscaling_target.metadata.resource_id
  scalable_dimension = aws_appautoscaling_target.metadata.scalable_dimension
  service_namespace  = aws_appautoscaling_target.metadata.service_namespace

  target_tracking_scaling_policy_configuration {
    target_value = var.target_autoscale_memory_utilization

    predefined_metric_specification {
      predefined_metric_type = "ECSServiceAverageMemoryUtilization"
    }

    scale_in_cooldown  = 300
    scale_out_cooldown = 300
  }
}

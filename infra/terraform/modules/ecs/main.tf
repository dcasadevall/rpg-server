# ECS Module

# Create ECS Cluster
resource "aws_ecs_cluster" "cluster" {
  name = "${var.project_name}-${var.environment}"

  setting {
    name  = "containerInsights"
    value = "enabled"
  }
}

resource "aws_ecs_cluster_capacity_providers" "cluster" {
  cluster_name = aws_ecs_cluster.cluster.name

  capacity_providers = ["FARGATE"]

  default_capacity_provider_strategy {
    base              = 1
    weight            = 100
    capacity_provider = "FARGATE"
  }
}

# Create Metadata Service
resource "aws_ecs_task_definition" "metadata" {
  family                   = "metadata-service"
  network_mode             = "awsvpc"
  requires_compatibilities = ["FARGATE"]
  cpu                      = 256
  memory                   = 512
  execution_role_arn       = aws_iam_role.ecs_task_execution_role.arn
  task_role_arn            = aws_iam_role.ecs_task_role.arn

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
          value = "dynamodb.${var.region}.amazonaws.com"
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

# Create Game Simulation Service
resource "aws_ecs_task_definition" "game_sim" {
  family                   = "game-sim-service"
  network_mode             = "awsvpc"
  requires_compatibilities = ["FARGATE"]
  cpu                      = 1024
  memory                   = 2048
  execution_role_arn       = aws_iam_role.ecs_task_execution_role.arn
  task_role_arn            = aws_iam_role.ecs_task_role.arn

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

# Create ECS Services
resource "aws_ecs_service" "metadata" {
  name            = "metadata-service"
  cluster         = aws_ecs_cluster.cluster.id
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

  depends_on = [aws_iam_role_policy_attachment.ecs_task_execution_role]
}

resource "aws_ecs_service" "game_sim" {
  name            = "game-sim-service"
  cluster         = aws_ecs_cluster.cluster.id
  task_definition = aws_ecs_task_definition.game_sim.arn
  desired_count   = var.game_sim_desired_capacity
  launch_type     = "FARGATE"

  network_configuration {
    subnets          = var.public_subnet_ids
    security_groups  = [aws_security_group.game_sim.id]
    assign_public_ip = true
  }

  depends_on = [aws_iam_role_policy_attachment.ecs_task_execution_role]
}

# Security Groups
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

# CloudWatch Log Groups
resource "aws_cloudwatch_log_group" "metadata" {
  name              = "/ecs/metadata-service"
  retention_in_days = 30
}

resource "aws_cloudwatch_log_group" "game_sim" {
  name              = "/ecs/game-sim-service"
  retention_in_days = 30
}

# IAM Roles
resource "aws_iam_role" "ecs_task_execution_role" {
  name = "ecs-task-execution-role"

  assume_role_policy = jsonencode({
    Version = "2012-10-17"
    Statement = [
      {
        Action = "sts:AssumeRole"
        Effect = "Allow"
        Principal = {
          Service = "ecs-tasks.amazonaws.com"
        }
      }
    ]
  })
}

resource "aws_iam_role" "ecs_task_role" {
  name = "ecs-task-role"

  assume_role_policy = jsonencode({
    Version = "2012-10-17"
    Statement = [
      {
        Action = "sts:AssumeRole"
        Effect = "Allow"
        Principal = {
          Service = "ecs-tasks.amazonaws.com"
        }
      }
    ]
  })
}

# Attach ECS Task Execution Role Policy
resource "aws_iam_role_policy_attachment" "ecs_task_execution_role" {
  role       = aws_iam_role.ecs_task_execution_role.name
  policy_arn = "arn:aws:iam::aws:policy/service-role/AmazonECSTaskExecutionRolePolicy"
}

# Attach DynamoDB access policy to task role
resource "aws_iam_role_policy" "dynamodb_access" {
  name = "dynamodb-access-policy"
  role = aws_iam_role.ecs_task_role.id

  policy = jsonencode({
    Version = "2012-10-17"
    Statement = [
      {
        Effect = "Allow"
        Action = [
          "dynamodb:GetItem",
          "dynamodb:PutItem",
          "dynamodb:UpdateItem",
          "dynamodb:DeleteItem",
          "dynamodb:Query",
          "dynamodb:Scan"
        ]
        Resource = "*"
      }
    ]
  })
}

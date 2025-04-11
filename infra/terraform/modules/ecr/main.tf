# ECR Module

# Create ECR repository for metadata service
resource "aws_ecr_repository" "metadata_service" {
  name = "${var.project_name}-metadata-service"
  force_delete = true

  image_scanning_configuration {
    scan_on_push = true
  }

  tags = {
    Name        = "${var.project_name}-metadata-service"
    Environment = var.environment
  }
}

# Create ECR repository for game simulation service
resource "aws_ecr_repository" "game_sim_service" {
  name = "${var.project_name}-game-sim-service"
  force_delete = true

  image_scanning_configuration {
    scan_on_push = true
  }

  tags = {
    Name        = "${var.project_name}-game-sim-service"
    Environment = var.environment
  }
}

# ECR Repository Policy
resource "aws_ecr_repository_policy" "repo_policy" {
  repository = aws_ecr_repository.metadata_service.name
  policy     = <<EOF
{
    "Version": "2008-10-17",
    "Statement": [
        {
            "Sid": "AllowPull",
            "Effect": "Allow",
            "Principal": "*",
            "Action": [
                "ecr:GetDownloadUrlForLayer",
                "ecr:BatchGetImage",
                "ecr:BatchCheckLayerAvailability",
                "ecr:PutImage",
                "ecr:InitiateLayerUpload",
                "ecr:UploadLayerPart",
                "ecr:CompleteLayerUpload"
            ]
        }
    ]
}
EOF
}

# ECR Repository Policy for game simulation service
resource "aws_ecr_repository_policy" "game_sim_repo_policy" {
  repository = aws_ecr_repository.game_sim_service.name
  policy     = <<EOF
{
    "Version": "2008-10-17",
    "Statement": [
        {
            "Sid": "AllowPull",
            "Effect": "Allow",
            "Principal": "*",
            "Action": [
                "ecr:GetDownloadUrlForLayer",
                "ecr:BatchGetImage",
                "ecr:BatchCheckLayerAvailability",
                "ecr:PutImage",
                "ecr:InitiateLayerUpload",
                "ecr:UploadLayerPart",
                "ecr:CompleteLayerUpload"
            ]
        }
    ]
}
EOF
}

data "aws_caller_identity" "current" {}

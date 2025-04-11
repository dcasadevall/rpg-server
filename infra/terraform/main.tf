# Amazon AWS Infra

# Declares AWS as the cloud provider
provider "aws" {
  region = var.region
}

# Main Infrastructure

# Deploy IAM resources first
module "iam" {
  source = "./modules/iam"

  iam_username = var.iam_username
}

# Deploy VPC and networking
module "vpc" {
  source = "./modules/vpc"

  environment = var.environment
  region      = var.region
  vpc_cidr    = var.environment == "dev" ? "10.0.0.0/16" : "10.1.0.0/16"
  tags = {
    Environment = var.environment
    Project     = var.project_name
    Management  = "terraform"
  }
}

# Creates the application load balancer (ALB) for our services
module "alb" {
  source            = "./modules/alb"
  vpc_id            = module.vpc.vpc_id
  public_subnet_ids = module.vpc.public_subnet_ids
  domain_name       = var.domain_name
}

# Deploy DynamoDB tables and IAM roles
module "dynamodb" {
  source = "./modules/dynamodb"

  project_name = var.project_name
  environment = var.environment
  characters_table_name = "characters"
  items_table_name = "items"
  billing_mode = "PAY_PER_REQUEST"
  read_capacity = var.dynamodb_read_capacity
  write_capacity = var.dynamodb_write_capacity
  ecs_task_role_id = module.ecs.ecs_task_role_id
  tags = {
    Environment = var.environment
    Project     = var.project_name
    Management  = "terraform"
  }
}

# Deploy ECR repositories for services
module "ecr" {
  source = "./modules/ecr"

  project_name = var.project_name
  environment  = var.environment
  tags         = var.tags
}

# Deploy Metadata Service
module "metadata_service" {
  source = "./modules/metadata-service"

  project_name = var.project_name
  environment  = var.environment
  vpc_id       = module.vpc.vpc_id
  public_subnet_ids = module.vpc.public_subnet_ids
  region       = var.region
  alb_security_group_id = module.alb.alb_security_group_id
  metadata_repository_url = module.ecr.metadata_repository_url
  metadata_target_group_arn = module.alb.metadata_target_group_arn
  metadata_desired_capacity = var.metadata_desired_capacity
  metadata_min_size = var.metadata_min_size
  metadata_max_size = var.metadata_max_size
  target_autoscale_cpu_utilization = var.target_metadata_autoscale_cpu_utilization
  target_autoscale_memory_utilization = var.target_metadata_autoscale_memory_utilization
  ecs_cluster_id = module.ecs.cluster_id
  ecs_cluster_name = "${var.project_name}-${var.environment}"
  ecs_task_execution_role_arn = module.ecs.ecs_task_execution_role_arn
  ecs_task_role_arn = module.ecs.ecs_task_role_arn
  ecs_task_execution_role_policy_attachment = module.ecs.ecs_task_execution_role_policy_attachment
}

# Deploy Game Simulation Service
module "game_sim_service" {
  source = "./modules/game-sim-service"

  project_name = var.project_name
  environment  = var.environment
  vpc_id       = module.vpc.vpc_id
  public_subnet_ids = module.vpc.public_subnet_ids
  region       = var.region
  ecs_cluster_id = module.ecs.cluster_id
  ecs_cluster_name = "${var.project_name}-${var.environment}"
  ecs_task_execution_role_arn = module.ecs.ecs_task_execution_role_arn
  ecs_task_role_arn = module.ecs.ecs_task_role_arn
  ecs_task_execution_role_policy_attachment = module.ecs.ecs_task_execution_role_policy_attachment
  game_sim_repository_url = module.ecr.game_sim_repository_url
  game_sim_udp_port = var.game_sim_udp_port
  game_sim_desired_capacity = var.game_sim_desired_capacity
  game_sim_min_size = var.game_sim_min_size
  game_sim_max_size = var.game_sim_max_size
  target_autoscale_session_ratio = var.target_autoscale_session_ratio
  metadata_service_dns = module.alb.alb_dns_name
  metadata_service_security_group_id = module.metadata_service.metadata_security_group_id
}

# Deploy ECS cluster and services
module "ecs" {
  source = "./modules/ecs"

  project_name = var.project_name
  environment  = var.environment
  region       = var.region
  vpc_id       = module.vpc.vpc_id
  public_subnet_ids = module.vpc.public_subnet_ids
  alb_security_group_id = module.alb.alb_security_group_id
  metadata_target_group_arn = module.alb.metadata_target_group_arn
  metadata_repository_url = module.ecr.metadata_repository_url
  game_sim_repository_url = module.ecr.game_sim_repository_url
  game_sim_udp_port = var.game_sim_udp_port
  game_sim_desired_capacity = var.game_sim_desired_capacity
  metadata_desired_capacity = var.metadata_desired_capacity
  metadata_service_dns = module.alb.alb_dns_name
  cluster_name = "${var.project_name}-${var.environment}"
}

# Deploy static content store
module "static_store" {
  source = "./modules/static_store"

  bucket_name = var.bucket_name
}

# Deploy DynamoDB seeder
module "dynamodb_seeder" {
  source = "./modules/dynamodb_seeder"

  environment = var.environment
  items_table_name = module.dynamodb.items_table_name
  region = var.region
}

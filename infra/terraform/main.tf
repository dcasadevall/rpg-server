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
  vpc_cidr    = var.environment == "dev" ? "10.0.0.0/16" : "10.1.0.0/16"
  tags = {
    Environment = var.environment
    Project     = var.project_name
    Management  = "terraform"
  }
}

# Creates the application load balancer (ALB) for our Metadata Service
module "alb" {
  source            = "./modules/alb"
  vpc_id            = module.vpc.vpc_id
  public_subnet_ids = module.vpc.public_subnet_ids
  domain_name       = var.domain_name
}

# Deploy DynamoDB tables and IAM roles
module "dynamodb" {
  source = "./modules/dynamodb"

  environment = var.environment
  characters_table_name = "characters"
  items_table_name = "items"
  billing_mode = "PAY_PER_REQUEST"
  read_capacity         = 5
  write_capacity        = 5
  tags = {
    Environment = var.environment
    Project     = var.project_name
    Management  = "terraform"
  }
}

# Deploy ECR repository for character service
module "ecr" {
  source = "./modules/ecr"

  project_name = var.project_name
  environment  = var.environment
  tags         = var.tags
}

# Deploy metadata service
module "metadata_service" {
  source = "./modules/metadata_service"

  vpc_id                      = module.vpc.vpc_id
  public_subnet_ids           = module.vpc.public_subnet_ids
  alb_security_group_id       = module.alb.alb_security_group_id
  alb_target_group_arn        = module.alb.alb_target_group_arn
  ami_id                      = var.metadata_ami_id
  instance_type               = var.instance_type
  min_size                    = var.environment == "dev" ? 1 : 2
  max_size                    = var.environment == "dev" ? 2 : 4
  desired_capacity            = var.environment == "dev" ? 1 : 2
  dynamodb_instance_profile_arn = module.dynamodb.instance_profile_arn
  dynamodb_prefix_list_id     = module.vpc.dynamodb_prefix_list_id
  user_data                   = var.metadata_user_data
  metadata_repository_url     = module.ecr.metadata_repository_url
  dynamodb_service_url        = "https://dynamodb.${var.region}.amazonaws.com"
  environment                 = var.environment
}

# Deploy game simulation service
module "game_sim_service" {
  source = "./modules/game_sim_service"

  vpc_id                          = module.vpc.vpc_id
  public_subnet_ids               = module.vpc.public_subnet_ids
  ami_id                          = var.gamesim_ami_id
  instance_type                   = var.game_sim_instance_type
  min_size                        = var.game_sim_min_size
  max_size                        = var.game_sim_max_size
  desired_capacity                = var.game_sim_desired_capacity
  udp_port                        = var.game_sim_udp_port
  environment                     = var.environment
  game_sim_repository_url         = module.ecr.game_sim_repository_url
  metadata_service_dns            = module.alb.alb_dns_name
  dynamodb_instance_profile_arn   = module.dynamodb.instance_profile_arn
  target_autoscale_session_ratio  = var.target_autoscale_session_ratio
}

module "static_store" {
  source = "./modules/static_store"

  bucket_name = var.bucket_name
}

module "dynamodb_seeder" {
  source = "./modules/dynamodb_seeder"

  environment = var.environment
  items_table_name = module.dynamodb.items_table_name
}

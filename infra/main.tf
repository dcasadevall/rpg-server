# Amazon AWS Infra

# Declares AWS as the cloud provider
provider "aws" {
  region = var.region
}

# Main Infrastructure

# Deploy VPC and networking
module "vpc" {
  source = "./modules/vpc"

  environment = "production"
  tags = {
    Environment = "production"
    Project     = "rpg-game"
    ManagedBy   = "terraform"
  }
}

# Creates the application load balancer (ALB) for our Metadata Service
module "alb" {
  source            = "./modules/alb"
  vpc_id            = module.vpc.vpc_id
  public_subnet_ids = module.vpc.public_subnet_ids
  # HTTPS Certificate for SSL
  certificate_arn   = var.certificate_arn
}

# Deploy DynamoDB tables and IAM roles
module "dynamodb" {
  source = "./modules/dynamodb"

  characters_table_name = "rpg-characters"
  items_table_name      = "rpg-items"
  billing_mode          = "PAY_PER_REQUEST"
  read_capacity         = 5
  write_capacity        = 5
  tags = {
    Environment = "production"
    Project     = "rpg-game"
    ManagedBy   = "terraform"
  }
}

# Deploy game simulation service first (needed for metadata service)
module "game_sim_service" {
  source            = "./modules/game_sim_service"
  vpc_id            = module.vpc.vpc_id
  public_subnet_ids = module.vpc.public_subnet_ids
  ami_id            = var.gamesim_ami_id
  instance_type     = var.gamesim_instance_type
  user_data         = var.gamesim_user_data
  udp_port          = var.gamesim_udp_port
  min_size          = 2
  max_size          = 20
  desired_capacity  = 4
}

# Deploy metadata service
module "metadata_service" {
  source = "./modules/metadata_service"

  vpc_id                     = module.vpc.vpc_id
  public_subnet_ids          = module.vpc.public_subnet_ids
  dynamodb_instance_profile_arn = module.dynamodb.instance_profile_arn
  ami_id                     = var.metadata_ami_id
  instance_type              = var.metadata_instance_type
  user_data                  = var.metadata_user_data
  alb_target_group_arn       = module.alb.alb_target_group_arn
  alb_security_group_id      = module.alb.alb_security_group_id
  gamesim_security_group_id  = module.game_sim_service.gamesim_security_group_id
  min_size                   = 2
  max_size                   = 10
  desired_capacity           = 2
}

module "static_store" {
  source      = "./modules/static_store"
  bucket_name = var.static_bucket_name
}

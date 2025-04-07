# Amazon AWS Infra

# Declares AWS as the cloud provider
provider "aws" {
  region = var.region
}

# Creates the application load balancer (ALB) for our Metadata Service
module "alb" {
  source            = "./modules/alb"
  vpc_id            = var.vpc_id
  public_subnet_ids = var.public_subnet_ids
  # HTTPS Certificate for SSL
  certificate_arn   = var.certificate_arn
}

# Deploys the Metadata Service
module "metadata_service" {
  source                    = "./modules/metadata_service"
  vpc_id                    = var.vpc_id
  public_subnet_ids         = var.public_subnet_ids
  ami_id                    = var.metadata_ami_id
  instance_type             = var.metadata_instance_type
  user_data                 = var.metadata_user_data
  alb_target_group_arn      = module.alb.alb_target_group_arn
  alb_security_group_id     = module.alb.alb_security_group_id
  gamesim_security_group_id = module.game_sim_service.gamesim_security_group_id
  min_size                  = 2
  max_size                  = 10
  desired_capacity          = 2
}

module "game_sim_service" {
  source            = "./modules/game_sim_service"
  vpc_id            = var.vpc_id
  public_subnet_ids = var.public_subnet_ids
  ami_id            = var.gamesim_ami_id
  instance_type     = var.gamesim_instance_type
  user_data         = var.gamesim_user_data
  udp_port          = var.gamesim_udp_port
  min_size          = 2
  max_size          = 20
  desired_capacity  = 4
}

module "rds_database" {
  source                   = "./modules/rds_database"
  vpc_id                   = var.vpc_id
  db_name                  = var.db_name
  db_username              = var.db_username
  db_password              = var.db_password
  allowed_security_groups  = [module.metadata_service.metadata_service_asg_name] # (adjust if needed)
  db_subnet_group_name     = var.db_subnet_group_name
}

module "static_store" {
  source      = "./modules/static_store"
  bucket_name = var.static_bucket_name
}

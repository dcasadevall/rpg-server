# RDS Database Module

# Create Security Group for RDS
resource "aws_security_group" "rds_sg" {
  name        = "rds-database-sg"
  description = "Allow database access from metadata services"
  vpc_id      = var.vpc_id

  ingress {
    from_port       = 5432
    to_port         = 5432
    protocol        = "tcp"
    security_groups = var.allowed_security_groups
  }

  egress {
    from_port   = 0
    to_port     = 0
    protocol    = "-1"
    cidr_blocks = ["0.0.0.0/0"]
  }
}

# Create RDS Instance
resource "aws_db_instance" "rds" {
  allocated_storage    = var.allocated_storage
  engine               = "postgres"
  engine_version       = var.engine_version
  instance_class       = var.instance_class
  db_name              = "fortis"
  username             = var.username
  password             = var.password
  vpc_security_group_ids = [aws_security_group.rds_sg.id]
  db_subnet_group_name   = var.db_subnet_group_name
  skip_final_snapshot    = true
} 
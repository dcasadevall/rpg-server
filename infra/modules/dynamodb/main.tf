# DynamoDB Module

# Create DynamoDB table for characters
resource "aws_dynamodb_table" "characters" {
  name           = var.characters_table_name
  billing_mode   = var.billing_mode
  hash_key       = "id"

  # Only set these if using PROVISIONED billing mode
  dynamic "read_capacity" {
    for_each = var.billing_mode == "PROVISIONED" ? [1] : []
    content {
      read_capacity_units = var.read_capacity
    }
  }

  dynamic "write_capacity" {
    for_each = var.billing_mode == "PROVISIONED" ? [1] : []
    content {
      write_capacity_units = var.write_capacity
    }
  }

  attribute {
    name = "id"
    type = "S"
  }

  tags = merge(var.tags, {
    Name = var.characters_table_name
  })
}

# Create DynamoDB table for items
resource "aws_dynamodb_table" "items" {
  name           = var.items_table_name
  billing_mode   = var.billing_mode
  hash_key       = "id"

  # Only set these if using PROVISIONED billing mode
  dynamic "read_capacity" {
    for_each = var.billing_mode == "PROVISIONED" ? [1] : []
    content {
      read_capacity_units = var.read_capacity
    }
  }

  dynamic "write_capacity" {
    for_each = var.billing_mode == "PROVISIONED" ? [1] : []
    content {
      write_capacity_units = var.write_capacity
    }
  }

  attribute {
    name = "id"
    type = "N"
  }

  tags = merge(var.tags, {
    Name = var.items_table_name
  })
}

# IAM role for EC2 instances
resource "aws_iam_role" "ec2_dynamodb_role" {
  name = "ec2_dynamodb_role"

  assume_role_policy = jsonencode({
    Version = "2012-10-17"
    Statement = [
      {
        Action = "sts:AssumeRole"
        Effect = "Allow"
        Principal = {
          Service = "ec2.amazonaws.com"
        }
      }
    ]
  })

  tags = var.tags
}

# IAM policy for DynamoDB access
resource "aws_iam_policy" "dynamodb_access" {
  name        = "dynamodb-access-policy"
  description = "Policy for accessing DynamoDB tables"

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
        Resource = [
          aws_dynamodb_table.characters.arn,
          aws_dynamodb_table.items.arn
        ]
      }
    ]
  })
}

# Attach the DynamoDB policy to the role
resource "aws_iam_role_policy_attachment" "dynamodb_policy_attachment" {
  role       = aws_iam_role.ec2_dynamodb_role.name
  policy_arn = aws_iam_policy.dynamodb_access.arn
}

# Create instance profile
resource "aws_iam_instance_profile" "ec2_dynamodb_profile" {
  name = "ec2_dynamodb_profile"
  role = aws_iam_role.ec2_dynamodb_role.name
}

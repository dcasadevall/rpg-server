# DynamoDB Seeder Module

# Create a Lambda function to seed the data
resource "aws_lambda_function" "dynamodb_seeder" {
  filename         = "${path.module}/seeder.zip"
  function_name    = "dynamodb-seeder-${var.environment}"
  role             = aws_iam_role.lambda_seeder.arn
  handler          = "seeder::seeder.Function::FunctionHandler"
  runtime          = "dotnet6"
  timeout          = 300
  memory_size      = 256

  environment {
    variables = {
      ITEMS_TABLE_NAME = var.items_table_name
      ENVIRONMENT      = var.environment
    }
  }
}

# IAM role for the Lambda function
resource "aws_iam_role" "lambda_seeder" {
  name = "dynamodb-seeder-role-${var.environment}"

  assume_role_policy = jsonencode({
    Version = "2012-10-17"
    Statement = [
      {
        Action = "sts:AssumeRole"
        Effect = "Allow"
        Principal = {
          Service = "lambda.amazonaws.com"
        }
      }
    ]
  })
}

# IAM policy for DynamoDB access
resource "aws_iam_policy" "lambda_dynamodb" {
  name        = "lambda-dynamodb-policy-${var.environment}"
  description = "Policy for Lambda to access DynamoDB"

  policy = jsonencode({
    Version = "2012-10-17"
    Statement = [
      {
        Effect = "Allow"
        Action = [
          "dynamodb:PutItem",
          "dynamodb:GetItem",
          "dynamodb:Query",
          "dynamodb:Scan"
        ]
        Resource = [
          "arn:aws:dynamodb:${data.aws_region.current.name}:${data.aws_caller_identity.current.account_id}:table/${var.items_table_name}-${var.environment}"
        ]
      }
    ]
  })
}

# Attach the DynamoDB policy to the role
resource "aws_iam_role_policy_attachment" "lambda_dynamodb" {
  role       = aws_iam_role.lambda_seeder.name
  policy_arn = aws_iam_policy.lambda_dynamodb.arn
}

# Basic Lambda execution policy
resource "aws_iam_role_policy_attachment" "lambda_basic" {
  role       = aws_iam_role.lambda_seeder.name
  policy_arn = "arn:aws:iam::aws:policy/service-role/AWSLambdaBasicExecutionRole"
}

data "aws_region" "current" {}
data "aws_caller_identity" "current" {}

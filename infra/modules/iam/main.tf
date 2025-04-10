# IAM Policy for Infrastructure Management
resource "aws_iam_policy" "infrastructure_management" {
  name        = "infrastructure-management-policy"
  description = "Policy for managing RPG game infrastructure"

  policy = jsonencode({
    Version = "2012-10-17"
    Statement = [
      {
        Effect = "Allow"
        Action = [
          # VPC permissions
          "ec2:CreateVpc",
          "ec2:CreateSubnet",
          "ec2:CreateRouteTable",
          "ec2:CreateRoute",
          "ec2:CreateInternetGateway",
          "ec2:AttachInternetGateway",
          "ec2:AssociateRouteTable",
          "ec2:Describe*",
          "ec2:ModifyVpcAttribute",
          "ec2:CreateLaunchTemplate",
          "ec2:CreateLaunchTemplateVersion",
          "ec2:DescribeLaunchTemplates",
          "ec2:CreateSecurityGroup",
          "ec2:AuthorizeSecurityGroupIngress",
          "ec2:AuthorizeSecurityGroupEgress",

          # DynamoDB permissions
          "dynamodb:CreateTable",
          "dynamodb:DescribeTable",
          "dynamodb:PutItem",
          "dynamodb:GetItem",
          "dynamodb:UpdateItem",
          "dynamodb:DeleteItem",
          "dynamodb:Query",
          "dynamodb:Scan",

          # IAM permissions
          "iam:CreateRole",
          "iam:CreatePolicy",
          "iam:AttachRolePolicy",
          "iam:PassRole",
          "iam:GetRole",
          "iam:GetPolicy",
          "iam:CreateInstanceProfile",
          "iam:AddRoleToInstanceProfile",

          # S3 permissions
          "s3:CreateBucket",
          "s3:PutBucketPolicy",
          "s3:PutBucketWebsite",
          "s3:PutObject",
          "s3:GetObject",
          "s3:ListBucket",

          # CloudFront permissions
          "cloudfront:CreateDistribution",
          "cloudfront:CreateCloudFrontOriginAccessIdentity",
          "cloudfront:GetDistribution",
          "cloudfront:GetCloudFrontOriginAccessIdentity",

          # ALB permissions
          "elasticloadbalancing:CreateLoadBalancer",
          "elasticloadbalancing:CreateTargetGroup",
          "elasticloadbalancing:CreateListener",
          "elasticloadbalancing:Describe*",

          # Auto Scaling permissions
          "autoscaling:CreateAutoScalingGroup",
          "autoscaling:CreateLaunchConfiguration",
          "autoscaling:CreateOrUpdateTags",
          "autoscaling:Describe*",
          "autoscaling:PutScalingPolicy",

          # CloudWatch permissions
          "cloudwatch:PutMetricAlarm",
          "cloudwatch:DescribeAlarms",
          "cloudwatch:DeleteAlarms"
        ]
        Resource = "*"
      }
    ]
  })
}

resource "aws_iam_user_policy_attachment" "infrastructure_management" {
  user       = var.iam_username
  policy_arn = aws_iam_policy.infrastructure_management.arn
}

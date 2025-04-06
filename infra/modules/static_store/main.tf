# Static Store Module (S3 + CloudFront)

# S3 Bucket
resource "aws_s3_bucket" "static_content" {
  bucket = var.bucket_name
  force_destroy = true
}

# S3 Bucket Policy (Public Read via CloudFront only)
resource "aws_s3_bucket_policy" "static_content_policy" {
  bucket = aws_s3_bucket.static_content.id
  policy = data.aws_iam_policy_document.s3_policy.json
}

data "aws_iam_policy_document" "s3_policy" {
  statement {
    principals {
      type        = "AWS"
      identifiers = ["*"]
    }

    actions = ["s3:GetObject"]

    resources = ["${aws_s3_bucket.static_content.arn}/*"]
  }
}

# CloudFront Distribution
resource "aws_cloudfront_distribution" "cdn" {
  origin {
    domain_name = aws_s3_bucket.static_content.bucket_regional_domain_name
    origin_id   = "static-content-origin"
  }

  enabled             = true
  default_root_object = "index.html"

  default_cache_behavior {
    viewer_protocol_policy = "redirect-to-https"
    allowed_methods        = ["GET", "HEAD"]
    cached_methods         = ["GET", "HEAD"]

    forwarded_values {
      query_string = false
      cookies {
        forward = "none"
      }
    }
    target_origin_id = "static-content-origin"
  }

  viewer_certificate {
    cloudfront_default_certificate = true
  }
} 
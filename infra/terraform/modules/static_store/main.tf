# Static Store Module (S3 + CloudFront)

provider "aws" {
  alias  = "static_store"
  region = "us-west-1"  # S3 bucket must be in us-west-1
}

# Generate random ID for bucket name
resource "random_id" "bucket_suffix" {
  byte_length = 4
}

# S3 Bucket for Static Content
resource "aws_s3_bucket" "static_store" {
  provider = aws.static_store
  bucket   = "${var.bucket_name}-${random_id.bucket_suffix.hex}"
}

resource "aws_s3_bucket_versioning" "static_store" {
  provider = aws.static_store
  bucket = aws_s3_bucket.static_store.id
  versioning_configuration {
    status = "Enabled"
  }
}

resource "aws_s3_bucket_server_side_encryption_configuration" "static_store" {
  provider = aws.static_store
  bucket = aws_s3_bucket.static_store.id

  rule {
    apply_server_side_encryption_by_default {
      sse_algorithm = "AES256"
    }
  }
}

# CloudFront Origin Access Identity
resource "aws_cloudfront_origin_access_identity" "oai" {
  comment = "OAI for ${var.bucket_name}"
}

# S3 Bucket Policy (Public Read via CloudFront only)
resource "aws_s3_bucket_policy" "static_content_policy" {
  provider = aws.static_store
  bucket = aws_s3_bucket.static_store.id
  policy = data.aws_iam_policy_document.s3_policy.json
}

data "aws_iam_policy_document" "s3_policy" {
  statement {
    principals {
      type        = "AWS"
      identifiers = [aws_cloudfront_origin_access_identity.oai.iam_arn]
    }

    actions = ["s3:GetObject"]

    resources = ["${aws_s3_bucket.static_store.arn}/*"]
  }
}

# CloudFront Distribution
resource "aws_cloudfront_distribution" "cdn" {
  enabled             = true
  is_ipv6_enabled     = true
  default_root_object = "index.html"
  price_class         = "PriceClass_100"

  origin {
    domain_name = aws_s3_bucket.static_store.bucket_regional_domain_name
    origin_id   = "static-content-origin"

    s3_origin_config {
      origin_access_identity = aws_cloudfront_origin_access_identity.oai.cloudfront_access_identity_path
    }
  }

  default_cache_behavior {
    allowed_methods  = ["GET", "HEAD", "OPTIONS"]
    cached_methods   = ["GET", "HEAD"]
    target_origin_id = "static-content-origin"

    forwarded_values {
      query_string = false
      cookies {
        forward = "none"
      }
    }

    viewer_protocol_policy = "redirect-to-https"
    min_ttl                = 0
    default_ttl            = 3600
    max_ttl                = 86400
  }

  restrictions {
    geo_restriction {
      restriction_type = "none"
    }
  }

  viewer_certificate {
    cloudfront_default_certificate = true
  }
}

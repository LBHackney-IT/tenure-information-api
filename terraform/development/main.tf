
# INSTRUCTIONS:
# 1) ENSURE YOU POPULATE THE LOCALS
# 2) ENSURE YOU REPLACE ALL INPUT PARAMETERS, THAT CURRENTLY STATE 'ENTER VALUE', WITH VALID VALUES
# 3) YOUR CODE WOULD NOT COMPILE IF STEP NUMBER 2 IS NOT PERFORMED!
# 4) ENSURE YOU CREATE A BUCKET FOR YOUR STATE FILE AND YOU ADD THE NAME BELOW - MAINTAINING THE STATE OF THE INFRASTRUCTURE YOU CREATE IS ESSENTIAL - FOR APIS, THE BUCKETS ALREADY EXIST
# 5) THE VALUES OF THE COMMON COMPONENTS THAT YOU WILL NEED ARE PROVIDED IN THE COMMENTS
# 6) IF ADDITIONAL RESOURCES ARE REQUIRED BY YOUR API, ADD THEM TO THIS FILE
# 7) ENSURE THIS FILE IS PLACED WITHIN A 'terraform' FOLDER LOCATED AT THE ROOT PROJECT DIRECTORY

terraform {
  required_providers {
    aws = {
      source  = "hashicorp/aws"
      version = "~> 3.0"
    }
  }
}

provider "aws" {
  region = "eu-west-2"
}

data "aws_caller_identity" "current" {}

data "aws_region" "current" {}

locals {
  parameter_store = "arn:aws:ssm:${data.aws_region.current.name}:${data.aws_caller_identity.current.account_id}:parameter"
  default_tags = {
    Name              = "tenure-information-api-${var.environment_name}"
    Environment       = var.environment_name
    terraform-managed = true
    project_name      = var.project_name
  }
}

terraform {
  backend "s3" {
    bucket  = "terraform-state-housing-development"
    encrypt = true
    region  = "eu-west-2"
    key     = "services/tenure-information-api/state"
  }
}

resource "aws_sns_topic" "tenure" {
  name                        = "tenure.fifo"
  fifo_topic                  = true
  content_based_deduplication = true
  kms_master_key_id           = "alias/aws/sns"
}

resource "aws_ssm_parameter" "tenure_sns_arn" {
  name  = "/sns-topic/development/tenure/arn"
  type  = "String"
  value = aws_sns_topic.tenure.arn
}

module "tenure_information_api_cloudwatch_dashboard" {
  source              = "github.com/LBHackney-IT/aws-hackney-common-terraform.git//modules/cloudwatch/dashboards/api-dashboard"
  environment_name    = var.environment_name
  api_name            = "tenure-information-api"
  sns_topic_name      = aws_sns_topic.tenure.name
  dynamodb_table_name = aws_dynamodb_table.tenureinformationapi_dynamodb_table.name
}

module "tenure_information_api_waf"{
  source              = "github.com/LBHackney-IT/aws-hackney-common-terraform.git//modules/waf"
  name                = "tenure-api-waf"
  scope               = "REGIONAL"
  default_action      {
    allow{}
  }
  visibility_config   {
      cloudwatch_metrics_enabled = false
      metric_name                = "tenure-api-waf-web-acl"
      sampled_requests_enabled   = false
  }
}

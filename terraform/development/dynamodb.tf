resource "aws_dynamodb_table" "tenureinformationapi_dynamodb_table" {
  name             = "TenureInformation"
  billing_mode     = "PAY_PER_REQUEST"
  hash_key         = "id"
  stream_enabled   = true
  stream_view_type = "NEW_AND_OLD_IMAGES"

  attribute {
    name = "id"
    type = "S"
  }

  tags = merge(
    local.default_tags,
    { BackupPolicy = "Dev" }
  )

  point_in_time_recovery {
    enabled = true
  }
}

data "aws_iam_policy_document" "assume_role" {
  statement {
    effect = "Allow"

    principals {
      type        = "Service"
      identifiers = ["lambda.amazonaws.com"]
    }

    actions = ["sts:AssumeRole"]
  }
}

data "aws_lambda_function" "dynamodb_stream_finance" {
  function_name = "housing-finance-interim-api-development"
}

data "aws_lambda_function" "dynamodb_stream_finance_new" {
  function_name = "housing-finance-interim-api-development-dynamodb-stream"
}

resource "aws_lambda_event_source_mapping" "aws_lambda_event_source" {
  event_source_arn  = aws_dynamodb_table.tenureinformationapi_dynamodb_table.stream_arn
  function_name     = data.aws_lambda_function.dynamodb_stream_finance_new.arn
  starting_position = "LATEST"
}



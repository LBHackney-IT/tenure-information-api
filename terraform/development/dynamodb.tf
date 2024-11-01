resource "aws_dynamodb_table" "tenureinformationapi_dynamodb_table" {
  name             = "TenureInformation"
  billing_mode     = "PAY_PER_REQUEST"
  hash_key         = "id"
  stream_enabled   = true
  stream_view_type = "KEYS_ONLY"

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

resource "aws_iam_role" "iam_for_lambda" {
  name               = "iam_for_lambda"
  assume_role_policy = data.aws_iam_policy_document.assume_role.json
}

resource "aws_lambda_function" "dynamodb_stream_poc" {
  function_name = "dynamodb-stream-poc"
  role          = aws_iam_role.iam_for_lambda.arn
  
}

# TO DO: Add lambda function name once created
resource "aws_lambda_event_source_mapping" "aws_lambda_event_source" {
  event_source_arn  = aws_dynamodb_table.tenureinformationapi_dynamodb_table.stream_arn
  function_name     = aws_lambda_function.dynamodb-stream-poc.arn
  starting_position = "LATEST"
}



resource "aws_cloudwatch_dashboard" "tenure_information_api_cw_dashboard" {

  dashboard_name = "tenure-information-api-${var.environment_name}-dashboard"
  dashboard_body = <<EOF
    {
    "widgets": [
        {
            "type": "metric",
            "x": 0,
            "y": 3,
            "width": 6,
            "height": 3,
            "properties": {
                "metrics": [
                    [ "AWS/ApiGateway", "5XXError", "ApiName", "${var.environment_name}-tenure-information-api" ]
                ],
                "view": "singleValue",
                "region": "eu-west-2",
                "period": 86400,
                "stat": "Sum",
                "title": "Number Of 5XX Errors"
            }
        },
        {
            "type": "metric",
            "x": 0,
            "y": 6,
            "width": 24,
            "height": 6,
            "properties": {
                "view": "timeSeries",
                "stacked": true,
                "metrics": [
                    [ "AWS/ApiGateway", "IntegrationLatency", "ApiName", "${var.environment_name}-tenure-information-api" ],
                    [ ".", "Latency", ".", "." ]
                ],
                "region": "eu-west-2"
            }
        },
        {
            "type": "metric",
            "x": 6,
            "y": 3,
            "width": 6,
            "height": 3,
            "properties": {
                "metrics": [
                    [ "AWS/ApiGateway", "4XXError", "ApiName", "${var.environment_name}-tenure-information-api" ]
                ],
                "view": "singleValue",
                "region": "eu-west-2",
                "period": 86400,
                "title": "Number Of 4XX Errors",
                "stat": "Sum"
            }
        },
        {
            "type": "metric",
            "x": 12,
            "y": 0,
            "width": 12,
            "height": 6,
            "properties": {
                "metrics": [
                    [ "AWS/ApiGateway", "Count", "ApiName", "${var.environment_name}-tenure-information-api" ],
                    [ ".", "5XXError", ".", "." ],
                    [ ".", "4XXError", ".", "." ]
                ],
                "view": "pie",
                "region": "eu-west-2",
                "period": 86400,
                "stat": "Average",
                "title": "Average of 4XXErrors, 5XXErrors and Request Counts"
            }
        },
        {
            "type": "metric",
            "x": 0,
            "y": 0,
            "width": 12,
            "height": 3,
            "properties": {
                "metrics": [
                    [ "AWS/ApiGateway", "Count", "ApiName", "${var.environment_name}-tenure-information-api" ]
                ],
                "view": "singleValue",
                "region": "eu-west-2",
                "period": 86400,
                "stat": "Sum",
                "title": "Total Request Count"
            }
        },
        {
            "type": "metric",
            "x": 0,
            "y": 12,
            "width": 18,
            "height": 6,
            "properties": {
                "view": "timeSeries",
                "stacked": false,
                "metrics": [
                    [ "AWS/Lambda", "Duration", "FunctionName", "tenure-information-api-${var.environment_name}", "Resource", "tenure-information-api-${var.environment_name}" ]
                ],
                "region": "eu-west-2",
                "title": "Tenure API Lambda Duration"
            }
        },
        {
            "type": "metric",
            "x": 18,
            "y": 15,
            "width": 6,
            "height": 3,
            "properties": {
                "metrics": [
                    [ "AWS/Lambda", "Errors", "FunctionName", "tenure-information-api-${var.environment_name}", "Resource", "tenure-information-api-${var.environment_name}" ]
                ],
                "view": "singleValue",
                "region": "eu-west-2",
                "period": 86400,
                "stat": "Sum",
                "title": "Tenure API  Lambda Errors"
            }
        },
        {
            "type": "metric",
            "x": 0,
            "y": 18,
            "width": 24,
            "height": 6,
            "properties": {
                "view": "timeSeries",
                "stacked": false,
                "metrics": [
                    [ "AWS/Lambda", "ConcurrentExecutions", "FunctionName", "tenure-information-api-${var.environment_name}", "Resource", "tenure-information-api-${var.environment_name}" ]
                ],
                "region": "eu-west-2",
                "title": "Tenure API Lambda ConcurrentExecutions"
            }
        },
        {
            "type": "metric",
            "x": 18,
            "y": 12,
            "width": 6,
            "height": 3,
            "properties": {
                "metrics": [
                    [ "AWS/Lambda", "Invocations", "FunctionName", "tenure-information-api-${var.environment_name}", "Resource", "tenure-information-api-${var.environment_name}" ]
                ],
                "view": "singleValue",
                "region": "eu-west-2",
                "period": 86400,
                "stat": "Sum",
                "title": "Tenure API Lambda Invocations"
            }
        }
    ]
}
EOF
}

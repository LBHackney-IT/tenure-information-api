module "tenure_information_api_cloudwatch_dashboard" {
  source           = "github.com/LBHackney-IT/aws-hackney-common-terraform.git//modules/cloudwatch/dashboards/api-dashboard"
  environment_name = var.environment_name
  api_name         = "tenure-information-api"
}

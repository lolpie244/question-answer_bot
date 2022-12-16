variable "aws_region" {
  description = "AWS region for all resources."
  nullable = false
  type     = string
  default  = "eu-central-1"
  validation {
    condition     = length(split("-", var.aws_region)) == 3
    error_message = "Invalid aws region."
  }
  sensitive = false
}

variable "lambda_timeout" {
  description = "Timeout for eu-central"
  type    = number
  default = 60
  validation {
    condition     = var.lambda_timeout > 0
    error_message = "Timeout cannot be less than 1"
  }
}
variable "server_type"{
  description = "Current server type"
  type    = string
  default = "Prod"
  validation {
	 condition     = contains(["Prod", "Dev"], var.server_type)
	 error_message = "Invalid server type"
  }
}

locals {
  memory_size = {
	Prod = 1532,
	Dev = 500
  }
}


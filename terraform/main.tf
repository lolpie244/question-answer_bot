# configure terraform
terraform {
  required_providers {
	# import module for connecting to aws
    aws = {
      source  = "hashicorp/aws"
      version = ">= 4.4.0"
    }
	# import module for rando
    random = {
      source  = "hashicorp/random"
      version = "~> 3.1.0"
    }
	# import module for archiving source files
    archive = {
      source  = "hashicorp/archive"
      version = "~> 2.2.0"
    }
  }

  required_version = "~> 1.0"
}

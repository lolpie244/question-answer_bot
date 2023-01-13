# init aws provider
provider "aws" {
  region = var.aws_region
}
# randoming name for s3 bucket
resource "random_pet" "lambda_bucket_name" {
  prefix = "frankobot"
  length = 4
}
# define bucket
resource "aws_s3_bucket" "lambda_bucket" {
  bucket = random_pet.lambda_bucket_name.id
}
# define bucket access control list
resource "aws_s3_bucket_acl" "bucket_acl" {
  bucket = aws_s3_bucket.lambda_bucket.id
  acl    = "private" # Owner gets FULL_CONTROL. No one else has access rights (default).
}

# packing project in zip
data "archive_file" "lambda_franko_bot" {
  type = "zip"

  source_dir  = "${path.module}/../franko_bot/bin/Release/net6.0/publish" # source directory
  output_path = "${path.module}/../franko_bot/bin/Release/net6.0/franko_bot.zip" # result archive
}

# define aws s3 object
resource "aws_s3_object" "lambda_franko_bot" {
  bucket = aws_s3_bucket.lambda_bucket.id

  key    = "franko_bot.zip"
  source = data.archive_file.lambda_franko_bot.output_path

  etag = filemd5(data.archive_file.lambda_franko_bot.output_path)
}

# configure lambda
resource "aws_lambda_function" "franko_bot" {
  function_name = "franko_bot_${var.server_type}"

  s3_bucket = aws_s3_bucket.lambda_bucket.id
  s3_key    = aws_s3_object.lambda_franko_bot.key

  timeout     = var.lambda_timeout 
  memory_size = local.memory_size[var.server_type]

  runtime = "dotnet6"
  handler = "franko_bot::franko_bot.LambdaEntryPoint::FunctionHandlerAsync"

  source_code_hash = data.archive_file.lambda_franko_bot.output_base64sha256

  role = aws_iam_role.lambda_exec.arn
}

# configure cloudwatch logging
resource "aws_cloudwatch_log_group" "franko_bot" {
  name = "/aws/lambda/${aws_lambda_function.franko_bot.function_name}"

  retention_in_days = 30
}


# define api gateway
resource "aws_api_gateway_rest_api" "gateway" {
  name        = "api_${var.server_type}"
}

# create {proxy+} resource
resource "aws_api_gateway_resource" "proxy" {
   rest_api_id = aws_api_gateway_rest_api.gateway.id
   parent_id   = aws_api_gateway_rest_api.gateway.root_resource_id
   path_part   = "{proxy+}"
}

# constraints for endpoint url/{proxy+}
resource "aws_api_gateway_method" "proxyMethod" {
   rest_api_id   = aws_api_gateway_rest_api.gateway.id
   resource_id   = aws_api_gateway_resource.proxy.id
   http_method   = "ANY"
   authorization = "NONE"
}

  
resource "aws_api_gateway_integration" "lambda" {
   rest_api_id = aws_api_gateway_rest_api.gateway.id
   resource_id = aws_api_gateway_method.proxyMethod.resource_id
   http_method = aws_api_gateway_method.proxyMethod.http_method

   integration_http_method = "POST"
   type                    = "AWS_PROXY"
   uri                     = aws_lambda_function.franko_bot.invoke_arn
}

# create root resource
resource "aws_api_gateway_method" "proxy_root" {
   rest_api_id   = aws_api_gateway_rest_api.gateway.id
   resource_id   = aws_api_gateway_rest_api.gateway.root_resource_id
   http_method   = "ANY"
   authorization = "NONE"
}


resource "aws_api_gateway_integration" "lambda_root" {
   rest_api_id = aws_api_gateway_rest_api.gateway.id
   resource_id = aws_api_gateway_method.proxy_root.resource_id
   http_method = aws_api_gateway_method.proxy_root.http_method

   integration_http_method = "POST"
   type                    = "AWS_PROXY"
   uri                     = aws_lambda_function.franko_bot.invoke_arn
}

#
resource "aws_api_gateway_deployment" "gateway" {
   depends_on = [
     aws_api_gateway_integration.lambda,
     aws_api_gateway_integration.lambda_root,
   ]

   rest_api_id = aws_api_gateway_rest_api.gateway.id
   stage_name  = var.server_type
}

#
resource "aws_lambda_permission" "apigw" {
   statement_id  = "AllowAPIGatewayInvoke"
   action        = "lambda:InvokeFunction"
   function_name = aws_lambda_function.franko_bot.function_name
   principal     = "apigateway.amazonaws.com"

   source_arn = "${aws_api_gateway_rest_api.gateway.execution_arn}/*/*"
}

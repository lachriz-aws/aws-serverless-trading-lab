using System.Collections.Generic;
using Amazon.CDK;
using Amazon.CDK.AWS.APIGateway;
using Amazon.CDK.AWS.IAM;
using Amazon.CDK.AWS.Lambda;
using Amazon.CDK.AWS.SSM;
using Constructs;

namespace ServerlessTrading.Infra.Stacks
{
    internal sealed class RestApiStack : Stack
    {
        internal RestApiStack(Construct scope, string id, IStackProps props = null) : base(scope, id, props)
        {
            /*
             * SSM Parameter Imports
             */
            var tradesTableName = StringParameter.FromStringParameterName(this, "tradesTableName", "/serverless-trading/config/trades-table-name").StringValue;

            /*
             * Trades REST API
             */
            var restApiProxyFunction = new Function(this, "restApiProxyFunction", new FunctionProps
            {
                Architecture = Architecture.ARM_64,
                Runtime = Runtime.DOTNET_6,
                FunctionName = "serverless-trading-rest-api-proxy",
                Description = "Serverless Trading - REST API",
                Handler = "ServerlessTrading.Api",
                Code = Code.FromAsset(@"../ServerlessTrading.Api/src/bin/lambda-package.zip"),
                MemorySize = 2048,
                Timeout = Duration.Seconds(29),
                Environment = new Dictionary<string, string>
                {
                    ["ASPNETCORE_ENVIRONMENT"] = "Production",
                    ["ServerlessTrading__Config__TradesTableName"] = tradesTableName
                },
                InitialPolicy = new[]
                {
                    new PolicyStatement(new PolicyStatementProps
                    {
                        Effect = Effect.ALLOW,
                        Actions = new []
                        {
                            "dynamodb:Scan",
                            "dynamodb:Query",
                            "dynamodb:PutItem",
                            "dynamodb:UpdateItem"
                        },
                        Resources = new []
                        {
                            $"arn:aws:dynamodb:{Region}:{Account}:table/serverless-trading-trades"
                        }
                    }),
                    new PolicyStatement(new PolicyStatementProps
                    {
                        Effect = Effect.ALLOW,
                        Actions = new []
                        {
                            "states:SendTaskFailure",
                            "states:SendTaskSuccess"
                        },
                        Resources = new []
                        {
                            $"arn:aws:states:{Region}:{Account}:stateMachine:serverless-trading-trade-approval-statemachine"
                        }
                    })
                }
            });

            var restApi = new LambdaRestApi(this, "restApi", new LambdaRestApiProps
            {
                RestApiName = "ServerlessTradingRestApi",
                Handler = restApiProxyFunction,
                Proxy = true,
                EndpointTypes = new[] { EndpointType.REGIONAL }
            });

            /*
             * Config
             */
            _ = new StringParameter(this, "ssmParamterRestApiBaseUrl", new StringParameterProps
            {
                ParameterName = "/serverless-trading/config/rest-api-base-url",
                Type = ParameterType.STRING,
                StringValue = restApi.Url
            });
        }
    }
}
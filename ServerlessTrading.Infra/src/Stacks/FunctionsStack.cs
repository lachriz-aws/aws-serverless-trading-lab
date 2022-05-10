using System.Collections.Generic;
using Amazon.CDK;
using Amazon.CDK.AWS.IAM;
using Amazon.CDK.AWS.Lambda;
using Amazon.CDK.AWS.SSM;
using Constructs;

namespace ServerlessTrading.Infra.Stacks
{
    internal sealed class FunctionsStack : Stack
    {
        internal FunctionsStack(Construct scope, string id, IStackProps props = null) : base(scope, id, props)
        {
            /*
             * SSM Parameter Imports
             */
            var tradesTableName = StringParameter.FromStringParameterName(this, "tradesTableName", "/serverless-trading/config/trades-table-name").StringValue;
            var snsNotificationTopicArn = StringParameter.FromStringParameterName(this, "snsNotificationTopicArn", "/serverless-trading/config/notification-topic-arn").StringValue;
            var tradingApiBaseUrl = StringParameter.FromStringParameterName(this, "tradingApiBaseUrl", "/serverless-trading/config/rest-api-base-url").StringValue;

            /*
             * Functions
             */
            var environmentVariables = new Dictionary<string, string>
            {
                ["DOTNET_ENVIRONMENT"] = "Production",
                ["ServerlessTrading__Config__TradesTableName"] = tradesTableName,
                ["ServerlessTrading__Config__SnsNotificationTopicArn"] = snsNotificationTopicArn,
                ["ServerlessTrading__Config__TradingApiBaseUrl"] = tradingApiBaseUrl
            };

            var createSampleTradesLambdaFunction = new Function(this, "createSampleTradesLambdaFunction", new FunctionProps
            {
                Architecture = Architecture.ARM_64,
                Runtime = Runtime.DOTNET_6,
                FunctionName = "serverless-trading-create-sample-trades",
                Description = "Serverless Trading - Create Sample Trades",
                Handler = "ServerlessTrading.Functions::ServerlessTrading.Functions.Handler::CreateSampleTradesAsync",
                Code = Code.FromAsset(@"../ServerlessTrading.Functions/src/bin/lambda-package.zip"),
                MemorySize = 2048,
                Timeout = Duration.Seconds(60),
                Environment = environmentVariables,
                InitialPolicy = new[]
                {
                    new PolicyStatement(new PolicyStatementProps
                    {
                        Effect = Effect.ALLOW,
                        Actions = new []
                        {
                            "dynamodb:PutItem"
                        },
                        Resources = new []
                        {
                            $"arn:aws:dynamodb:{Region}:{Account}:table/serverless-trading-trades"
                        }
                    })
                }
            });

            var startManualTradeApprovalLambdaFunction = new Function(this, "startManualTradeApprovalLambdaFunction", new FunctionProps
            {
                Architecture = Architecture.ARM_64,
                Runtime = Runtime.DOTNET_6,
                FunctionName = "serverless-trading-start-manual-trade-approval",
                Description = "Serverless Trading - Start Manual Trade Approval",
                Handler = "ServerlessTrading.Functions::ServerlessTrading.Functions.Handler::StartManualTradeApprovalAsync",
                Code = Code.FromAsset(@"../ServerlessTrading.Functions/src/bin/lambda-package.zip"),
                MemorySize = 2048,
                Timeout = Duration.Seconds(60),
                Environment = environmentVariables,
                InitialPolicy = new[]
                {
                    new PolicyStatement(new PolicyStatementProps
                    {
                        Effect = Effect.ALLOW,
                        Actions = new []
                        {
                            "dynamodb:Query"
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
                            "sns:Publish"
                        },
                        Resources = new []
                        {
                            $"arn:aws:sns:{Region}:{Account}:serverless-trading-notification-topic"
                        }
                    })
                }
            });

            /*
             * Config
             */
            _ = new StringParameter(this, "createSampleTradesLambdaFunctionArn", new StringParameterProps
            {
                ParameterName = "/serverless-trading/config/create-sample-trades-lambda-function-arn",
                Type = ParameterType.STRING,
                StringValue = createSampleTradesLambdaFunction.FunctionArn
            });
            _ = new StringParameter(this, "startManualTradeApprovalLambdaFunctionArn", new StringParameterProps
            {
                ParameterName = "/serverless-trading/config/start-manual-trade-approval-lambda-function-arn",
                Type = ParameterType.STRING,
                StringValue = startManualTradeApprovalLambdaFunction.FunctionArn
            });
        }
    }
}
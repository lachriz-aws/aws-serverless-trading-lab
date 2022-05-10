using Amazon.CDK;
using Amazon.CDK.AWS.DynamoDB;
using Amazon.CDK.AWS.SSM;
using Constructs;

namespace ServerlessTrading.Infra.Stacks
{
    internal sealed class StorageStack : Stack
    {
        internal StorageStack(Construct scope, string id, IStackProps props = null) : base(scope, id, props)
        {
            /*
             * Trades table
             */
            var tradesTable = new Table(this, "tradesTable", new TableProps
            {
                BillingMode = BillingMode.PAY_PER_REQUEST,
                Encryption = TableEncryption.AWS_MANAGED,
                PartitionKey = new Attribute
                {
                    Name = "trade_id",
                    Type = AttributeType.STRING
                },
                TableName = "serverless-trading-trades"
            });

            /*
             * Config
             */
            _ = new StringParameter(this, "ssmParamterTradesTableArn", new StringParameterProps
            {
                ParameterName = "/serverless-trading/config/trades-table-arn",
                Type = ParameterType.STRING,
                StringValue = tradesTable.TableArn
            });
            _ = new StringParameter(this, "ssmParamterTradesTableName", new StringParameterProps
            {
                ParameterName = "/serverless-trading/config/trades-table-name",
                Type = ParameterType.STRING,
                StringValue = tradesTable.TableName
            });
        }
    }
}
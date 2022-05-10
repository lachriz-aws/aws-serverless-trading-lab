using Amazon.CDK;
using ServerlessTrading.Infra.Stacks;

namespace ServerlessTrading.Infra
{
    internal sealed class Program
    {
        public static void Main()
        {
            var app = new App();
            _ = new StorageStack(app, "storage", new StackProps
            {
                StackName = "serverless-trading-storage-stack"
            });
            _ = new NotificationsStack(app, "notifications", new StackProps
            {
                StackName = "serverless-trading-notifications-stack"
            });
            _ = new FunctionsStack(app, "functions", new StackProps
            {
                StackName = "serverless-trading-functions-stack"
            });
            _ = new RestApiStack(app, "rest-api", new StackProps
            {
                StackName = "serverless-trading-rest-api-stack"
            });
            _ = new WorkflowStack(app, "workflow", new StackProps
            {
                StackName = "serverless-trading-workflow-stack"
            });
            app.Synth();
        }
    }
}

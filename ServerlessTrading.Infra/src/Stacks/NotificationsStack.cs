using Amazon.CDK;
using Amazon.CDK.AWS.SNS;
using Amazon.CDK.AWS.SSM;
using Constructs;

namespace ServerlessTrading.Infra.Stacks
{
    internal sealed class NotificationsStack : Stack
    {
        internal NotificationsStack(Construct scope, string id, IStackProps props = null) : base(scope, id, props)
        {
            /*
             * Notifications
             */
            var notificationTopic = new Topic(this, "notificationTopic", new TopicProps
            {
                TopicName = "serverless-trading-notification-topic"
            });

            /*
             * Config
             */
            _ = new StringParameter(this, "ssmParamterNotificationTopicArn", new StringParameterProps
            {
                ParameterName = "/serverless-trading/config/notification-topic-arn",
                Type = ParameterType.STRING,
                StringValue = notificationTopic.TopicArn
            });
            _ = new StringParameter(this, "ssmParamterNotificationTopicName", new StringParameterProps
            {
                ParameterName = "/serverless-trading/config/notification-topic-name",
                Type = ParameterType.STRING,
                StringValue = notificationTopic.TopicName
            });
        }
    }
}
using System.Collections.Generic;
using Amazon.CDK;
using Amazon.CDK.AWS.Lambda;
using Amazon.CDK.AWS.SSM;
using Amazon.CDK.AWS.StepFunctions;
using Amazon.CDK.AWS.StepFunctions.Tasks;
using Constructs;

namespace ServerlessTrading.Infra.Stacks
{
    internal sealed class WorkflowStack : Stack
    {
        internal WorkflowStack(Construct scope, string id, IStackProps props = null) : base(scope, id, props)
        {
            /*
             * SSM Parameter Imports
             */
            var startManualTradeApprovalLambdaFunctionArn = StringParameter.FromStringParameterName(
                this, "startManualTradeApprovalLambdaFunctionArn", "/serverless-trading/config/start-manual-trade-approval-lambda-function-arn").StringValue;

            /*
             * States
             */
            var startManualApprovalState = new LambdaInvoke(this, "startManualApprovalState", new LambdaInvokeProps
            {
                LambdaFunction = Function.FromFunctionArn(this, "startManualTradeApprovalLambdaFunction", startManualTradeApprovalLambdaFunctionArn),
                Payload = TaskInput.FromObject(new Dictionary<string, object>
                {
                    {"Token", JsonPath.TaskToken},
                    {"Payload", JsonPath.EntirePayload}
                }),
                IntegrationPattern = IntegrationPattern.WAIT_FOR_TASK_TOKEN
            });
            var approvedState = new Succeed(this, "approvedState");
            var rejectedState = new Succeed(this, "rejectedState");
            var handleApprovalResponseState = new Choice(this, "handleApprovalResponseState")
                .When(Condition.StringEquals("$.Action", "approve"), approvedState)
                .Otherwise(rejectedState);

            startManualApprovalState
                .Next(handleApprovalResponseState);

            /*
             * State Machine
             */
            var tradeApprovalStateMachine = new StateMachine(this, "tradeApprovalStateMachine", new StateMachineProps
            {
                Definition = startManualApprovalState,
                StateMachineName = "serverless-trading-trade-approval-statemachine",
                StateMachineType = StateMachineType.STANDARD
            });

            /*
             * Config
             */
            _ = new StringParameter(this, "ssmParamterTradeApprovalStateMachineArn", new StringParameterProps
            {
                ParameterName = "/serverless-trading/config/trade-approval-statemachine-arn",
                Type = ParameterType.STRING,
                StringValue = tradeApprovalStateMachine.StateMachineArn
            });
        }
    }
}
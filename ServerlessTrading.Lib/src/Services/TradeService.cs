using System.Text.Json;
using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DocumentModel;
using Amazon.DynamoDBv2.Model;
using Amazon.StepFunctions;
using Amazon.StepFunctions.Model;
using Microsoft.Extensions.Options;
using ServerlessTrading.Entities;
using ServerlessTrading.Lib.Extensions;

namespace ServerlessTrading.Lib.Services
{
    public class TradeService
    {
        private readonly TradeServiceOptions _options;
        private readonly IAmazonDynamoDB _dynamoClient;
        private readonly IAmazonStepFunctions _sfClient;

        public TradeService(IOptions<TradeServiceOptions> options, IAmazonDynamoDB dynamoClient, IAmazonStepFunctions sfClient)
        {
            _options = options.Value;
            _dynamoClient = dynamoClient;
            _sfClient = sfClient;
        }

        public async Task<List<TradeEntity>> GetTradesAsync()
        {
            var result = new List<TradeEntity>();
            var paginator = _dynamoClient.Paginators.Scan(new ScanRequest(_options.TradesTableName));
            await foreach (var response in paginator.Responses)
            {
                var games = response.Items.Select(x => JsonSerializer.Deserialize<TradeEntity>(Document.FromAttributeMap(x).ToJson()));
                result.AddRange(games!);
            }
            return result;
        }

        public async Task<TradeEntity?> GetTradeAsync(string? tradeId)
        {
            var response = await _dynamoClient.QueryAsync(new QueryRequest(_options.TradesTableName)
            {
                KeyConditionExpression = "#TradeId = :TradeId",
                ExpressionAttributeNames = new Dictionary<string, string>
                {
                    ["#TradeId"] = "trade_id"
                },
                ExpressionAttributeValues = new Dictionary<string, AttributeValue>
                {
                    [":TradeId"] = new(tradeId)
                }
            });
            var item = response.Items.FirstOrDefault();
            return item == null
                ? null
                : JsonSerializer.Deserialize<TradeEntity>(Document.FromAttributeMap(item).ToJson());
        }

        public async Task<TradeEntity> PutTradeAsync(string? tradeCurrency, string? tradeType, int traderId, int tradeAmount)
        {
            var tradeEntity = new TradeEntity
            {
                TradeAmount = tradeAmount,
                TradeCurrency = tradeCurrency,
                TradeDate = DateTime.UtcNow.ToIso8601(),
                TradeId = $"{Guid.NewGuid():N}",
                TradeType = tradeType,
                TraderId = traderId
            };
            var putItemRequest = new PutItemRequest
            {
                TableName = _options.TradesTableName,
                Item = Document.FromJson(JsonSerializer.Serialize(tradeEntity)).ToAttributeMap(),
                ReturnValues = ReturnValue.NONE
            };
            await _dynamoClient.PutItemAsync(putItemRequest);
            return tradeEntity;
        }

        public async Task RegisterApprovalActionAsync(string? tradeId, string? action, string? token)
        {
            // Set approval status
            await _dynamoClient.UpdateItemAsync(new UpdateItemRequest
            {
                TableName = _options.TradesTableName,
                UpdateExpression = "SET #ApprovalAction = :ApprovalAction, #ApprovalActionDate = :ApprovalActionDate",
                Key = new Dictionary<string, AttributeValue>
                {
                    ["trade_id"] = new(tradeId)
                },
                ExpressionAttributeNames = new Dictionary<string, string>
                {
                    ["#ApprovalAction"] = "trade_appr_action",
                    ["#ApprovalActionDate"] = "trade_appr_date",
                },
                ExpressionAttributeValues = new Dictionary<string, AttributeValue>
                {
                    [":ApprovalAction"] = new(action),
                    [":ApprovalActionDate"] = new($"{DateTime.UtcNow.ToIso8601()}")
                },
                ReturnValues = ReturnValue.ALL_NEW
            });

            // Continue workflow
            var taskSuccessRequest = new SendTaskSuccessRequest
            {
                TaskToken = token,
                Output = JsonSerializer.Serialize(new
                {
                    Action = action
                })
            };
            await _sfClient.SendTaskSuccessAsync(taskSuccessRequest);
        }
    }
}

using System.Text;
using System.Web;
using Amazon.SimpleNotificationService;
using Amazon.SimpleNotificationService.Model;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using ServerlessTrading.Entities;

namespace ServerlessTrading.Lib.Services
{
    public class NotificationService
    {
        private readonly ILogger<NotificationService> _logger;
        private readonly NotificationServiceOptions _options;
        private readonly IAmazonSimpleNotificationService _snsClient;

        public NotificationService(
            ILogger<NotificationService> logger,
            IOptions<NotificationServiceOptions> options,
            IAmazonSimpleNotificationService snsClient)
        {
            _logger = logger;
            _options = options.Value;
            _snsClient = snsClient;
        }

        public async Task PublishRequestForManualTradeApprovalAsync(TradeEntity trade, string? token)
        {
            // Build message
            var tradingApiBaseUrl = _options.TradingApiBaseUrl?.TrimEnd('/');
            var tradeInfoUrl = $"{tradingApiBaseUrl}/trades/{HttpUtility.UrlEncode(trade.TradeId)}";
            var approveUrl = $"{tradingApiBaseUrl}/webhooks/trade-approval?action=approve" +
                             $"&tradeId={HttpUtility.UrlEncode(trade.TradeId)}" +
                             $"&token={HttpUtility.UrlEncode(token)}";
            var rejectUrl = $"{tradingApiBaseUrl}/webhooks/trade-approval?action=reject" +
                            $"&tradeId={HttpUtility.UrlEncode(trade.TradeId)}" +
                            $"&token={HttpUtility.UrlEncode(token)}";

            var sb = new StringBuilder();
            sb.AppendLine($"Serverless Trading - Approval request for trade: '{trade.TradeId}'");
            sb.AppendLine();
            sb.AppendLine("Click here to see trade info:");
            sb.AppendLine(tradeInfoUrl);
            sb.AppendLine();
            sb.AppendLine("Click here to approve:");
            sb.AppendLine(approveUrl);
            sb.AppendLine();
            sb.AppendLine("Click here to reject:");
            sb.AppendLine(rejectUrl);
            sb.AppendLine();
            sb.AppendLine("--");
            sb.AppendLine("Powered by Serverless Trading");
            var message = sb.ToString();

            _logger.LogInformation("Message to be published:");
            _logger.LogInformation(message);

            // Publish notification
            var publishRequest = new PublishRequest
            {
                TopicArn = _options.SnsNotificationTopicArn,
                Message = message
            };
            await _snsClient.PublishAsync(publishRequest);
        }
    }
}

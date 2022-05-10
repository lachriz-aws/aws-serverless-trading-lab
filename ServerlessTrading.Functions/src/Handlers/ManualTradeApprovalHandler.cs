using Microsoft.Extensions.Logging;
using ServerlessTrading.Functions.Models;
using ServerlessTrading.Lib.Services;

namespace ServerlessTrading.Functions.Handlers
{
    internal class ManualTradeApprovalHandler
    {
        private readonly ILogger<ManualTradeApprovalHandler> _logger;
        private readonly TradeService _tradeService;
        private readonly NotificationService _notificationService;

        public ManualTradeApprovalHandler(
            ILogger<ManualTradeApprovalHandler> logger,
            TradeService tradeService,
            NotificationService notificationService)
        {
            _logger = logger;
            _tradeService = tradeService;
            _notificationService = notificationService;
        }

        public async Task RunAsync(TaskToken<RequestTradeApprovalMessage> request)
        {
            _logger.LogInformation("Received request to start manual trade approval...");

            // Get trade
            var trade = await _tradeService.GetTradeAsync(request.Payload?.TradeId);
            if (trade == null)
            {
                throw new InvalidOperationException($"Could not find trade with id: '{request.Payload?.TradeId}'");
            }

            // Request manual approval
            await _notificationService.PublishRequestForManualTradeApprovalAsync(trade, request.Token);
        }
    }
}

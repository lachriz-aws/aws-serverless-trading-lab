using Microsoft.Extensions.Logging;
using ServerlessTrading.Entities;
using ServerlessTrading.Functions.Models;
using ServerlessTrading.Lib.Services;

namespace ServerlessTrading.Functions.Handlers
{
    internal class CreateSampleTradesHandler
    {
        private readonly ILogger<CreateSampleTradesHandler> _logger;
        private readonly TradeService _tradeService;
        
        private static readonly Random Random = new();

        public CreateSampleTradesHandler(ILogger<CreateSampleTradesHandler> logger, TradeService tradeService)
        {
            _logger = logger;
            _tradeService = tradeService;
        }

        public async Task<CreateSampleTradesResponse> RunAsync(CreateSampleTradesRequest request)
        {
            // Guardrail
            request.NumberOfTrades = Math.Min(request.NumberOfTrades, 20);

            // Log action
            _logger.LogInformation($"Received request to create: '{request.NumberOfTrades}' sample trade(s)...");

            // Create data
            var trades = new List<TradeEntity>();
            var currencies = new[] { "DKK", "SEK", "NOK" };
            var tradeTypes = new[] { "FSX1", "FSX2", "FSX3" };

            for (var i = 0; i < request.NumberOfTrades; i++)
            {
                var tradeCurrency = currencies[Random.Next(currencies.Length)];
                var tradeType = tradeTypes[Random.Next(tradeTypes.Length)];
                var traderId = Random.Next(1, 1000);
                var tradeAmount = Random.Next(100, 10000);
                var trade = await _tradeService.PutTradeAsync(tradeCurrency, tradeType, traderId, tradeAmount);
                trades.Add(trade);
            }

            // Build response
            var response = new CreateSampleTradesResponse
            {
                Trades = trades
            };
            return response;
        }
    }
}

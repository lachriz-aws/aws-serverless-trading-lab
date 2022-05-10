using Microsoft.AspNetCore.Mvc;
using ServerlessTrading.Api.Models;
using ServerlessTrading.Lib.Services;

namespace ServerlessTrading.Api.Controllers
{
    [ApiController]
    [Route("trades")]
    public class TradesController : ControllerBase
    {
        private readonly ILogger<TradesController> _logger;
        private readonly TradeService _tradeService;

        public TradesController(ILogger<TradesController> logger, TradeService tradeService)
        {
            _logger = logger;
            _tradeService = tradeService;
        }

        [HttpGet("")]
        public async Task<IActionResult> GetTrades()
        {
            _logger.LogInformation("Getting trades...");
            var trades = await _tradeService.GetTradesAsync();
            var response = new GetTradesResponse
            {
                Trades = trades
            };
            return Ok(response);
        }

        [HttpGet("{tradeId}")]
        public async Task<IActionResult> GetTrade(string tradeId)
        {
            _logger.LogInformation($"Getting trade with id: '{tradeId}'");
            var trade = await _tradeService.GetTradeAsync(tradeId);
            if (trade == null)
            {
                return NotFound($"Trade with id: '{tradeId}' not found.");
            }
            var response = new GetTradeResponse
            {
                Trade = trade
            };
            return Ok(response);
        }

        [HttpPost("")]
        public async Task<IActionResult> CreateTrade([FromBody]CreateTradeRequest createTradeRequest)
        {
            _logger.LogInformation("Creating trade...");
            var trade = await _tradeService.PutTradeAsync(
                createTradeRequest.TradeCurrency,
                createTradeRequest.TradeType,
                createTradeRequest.TraderId ?? 0,
                createTradeRequest.TradeAmount ?? 0);
            var response = new CreateTradeResponse
            {
                Trade = trade
            };
            return Ok(response);
        }
    }
}
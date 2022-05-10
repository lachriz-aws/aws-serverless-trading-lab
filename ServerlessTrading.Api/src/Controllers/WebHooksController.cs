using Microsoft.AspNetCore.Mvc;
using ServerlessTrading.Api.Models;
using ServerlessTrading.Lib.Services;

namespace ServerlessTrading.Api.Controllers
{
    [ApiController]
    [Route("webhooks")]
    public class WebHooksController : ControllerBase
    {
        private readonly ILogger<WebHooksController> _logger;
        private readonly TradeService _tradeService;

        public WebHooksController(ILogger<WebHooksController> logger, TradeService tradeService)
        {
            _logger = logger;
            _tradeService = tradeService;
        }

        [HttpGet("trade-approval")]
        public async Task<IActionResult> HandleTradeApprovalActionEvent(
            [FromQuery(Name = "tradeId")] string tradeId,
            [FromQuery(Name = "action")] string action,
            [FromQuery(Name = "token")] string token)
        {
            _logger.LogInformation($"Registering trade approval action: '{action}' for trade with id: '{tradeId}'");

            await _tradeService.RegisterApprovalActionAsync(tradeId, action, token);

            var response = new TradeApprovalActionEventResponse
            {
                TradeId = tradeId,
                Action = action
            };
            return Ok(response);
        }
    }
}

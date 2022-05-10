using ServerlessTrading.Entities;

namespace ServerlessTrading.Api.Models
{
    public class GetTradesResponse
    {
        public List<TradeEntity>? Trades { get; set; }
    }
}

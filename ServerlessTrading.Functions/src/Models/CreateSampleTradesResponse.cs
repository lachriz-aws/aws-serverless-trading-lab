using ServerlessTrading.Entities;

namespace ServerlessTrading.Functions.Models
{
    public class CreateSampleTradesResponse
    {
        public List<TradeEntity>? Trades { get; set; }
    }
}

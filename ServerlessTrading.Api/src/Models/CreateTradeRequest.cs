namespace ServerlessTrading.Api.Models
{
    public class CreateTradeRequest
    {
        public string? TradeCurrency { get; set; }
        public string? TradeType { get; set; }
        public int? TraderId { get; set; }
        public int? TradeAmount { get; set; }
    }
}

using System.Text.Json.Serialization;

namespace ServerlessTrading.Entities
{
    public class TradeEntity
    {
        [JsonPropertyName("trade_ccy")]
        public string? TradeCurrency { get; set; }
        [JsonPropertyName("trade_date")]
        public string? TradeDate { get; set; }
        [JsonPropertyName("trade_type")]
        public string? TradeType { get; set; }
        [JsonPropertyName("trader_id")]
        public int TraderId { get; set; }
        [JsonPropertyName("trade_amount")]
        public int TradeAmount { get; set; }
        [JsonPropertyName("trade_id")]
        public string? TradeId { get; set; }
        [JsonPropertyName("trade_appr_action")]
        public string? TradeApprovalAction { get; set; }
        [JsonPropertyName("trade_appr_date")]
        public string? TradeApprovalDate { get; set; }
    }
}
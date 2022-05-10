namespace ServerlessTrading.Lib.Services
{
    public class NotificationServiceOptions
    {
        public string? SnsNotificationTopicArn { get; set; }
        public string? TradingApiBaseUrl { get; set; }
    }
}

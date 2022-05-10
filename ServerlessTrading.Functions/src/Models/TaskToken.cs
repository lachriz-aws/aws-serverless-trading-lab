namespace ServerlessTrading.Functions.Models
{
    public class TaskToken<TPayload>
    {
        public string? Token { get; set; }
        public TPayload? Payload { get; set; }
    }
}

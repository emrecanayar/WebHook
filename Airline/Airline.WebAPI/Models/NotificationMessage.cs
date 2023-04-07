namespace Airline.WebAPI.Models
{
    public class NotificationMessage
    {
        public string WebhookType { get; set; }
        public string Code { get; set; } = default!;
        public decimal NewPrice { get; set; } = default!;
    }
}

namespace Airline.WebAPI.Entities
{
    //Subscription bilgilerinin tutulacağı alan. Yani Airline nın paylaşmış olduğu bilgilere abone olanların bilgisinin tutulduğu alan.
    public class WebhookSubscription
    {
        public int Id { get; set; }
        public string WebhookUri { get; set; } = default!;
        public Guid Secret { get; set; } = default!;
        public string WebhookType { get; set; } = default!;
        public string WebhookPublisher { get; set; } = default!;
    }
}

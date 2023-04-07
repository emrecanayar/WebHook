namespace Airline.WebAPI.Dtos
{
    public class WebhookSubscriptionReadDto
    {
        public int Id { get; set; }
        public string WebHookURI { get; set; }
        public Guid Secret { get; set; }
        public string WebHookType { get; set; }
        public string WebHookPublisher { get; set; }
    }
}
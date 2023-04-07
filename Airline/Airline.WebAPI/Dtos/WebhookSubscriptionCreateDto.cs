using System.ComponentModel.DataAnnotations;

namespace Airline.WebAPI.Dtos
{
    public class WebhookSubscriptionCreateDto
    {
        [Required]
        public string WebHookURI { get; set; }
        [Required]
        public string WebHookType { get; set; }
    }
}
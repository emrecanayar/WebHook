using Airline.WebAPI.Dtos;
using Airline.WebAPI.Entities;
using AutoMapper;

namespace Airline.WebAPI.Profiles
{
    public class WebhookSubscriptionProfile : Profile
    {
        public WebhookSubscriptionProfile()
        {
            CreateMap<WebhookSubscription, WebhookSubscriptionReadDto>().ReverseMap();
            CreateMap<WebhookSubscription, WebhookSubscriptionCreateDto>().ReverseMap();
        }

    }
}

using Airline.WebAPI.Contexts;
using Airline.WebAPI.Dtos;
using Airline.WebAPI.Entities;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;

namespace Airline.WebAPI.Controllers
{
    [ApiController, Route("api/[controller]")]
    public class WebhookSubscriptionsController : ControllerBase
    {
        private readonly AirlineDbContext _context;
        private readonly IMapper _mapper;

        public WebhookSubscriptionsController(AirlineDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        // Abone olduktan sonra elde edilen secret bilgiyle ilgili aboneliği sorgulayabiliyoruz. Yani aboneliğimiz var mı yok mu bunu secret olarak bize verilen bilgi ile sorgulayabiliyoruz.
        [HttpGet("{secret}", Name = "GetSubscriptionBySecret")]
        public ActionResult<WebhookSubscriptionReadDto> GetSubscriptionBySecret(Guid secret)
        {
            var subscription = _context.WebhookSubscriptions.FirstOrDefault(s => s.Secret == secret);
            if (subscription == null)
                return NotFound();

            return Ok(_mapper.Map<WebhookSubscriptionCreateDto>(subscription));
        }

        //Burada Airline Servisine bir subscription işlemi gerçekleştiriliyor. Yani yayın yapacak olan servisi abone olma işlemi burada yapılıyor.
        [HttpPost]
        public ActionResult<WebhookSubscriptionReadDto> CreateSubscription(WebhookSubscriptionCreateDto webhookSubscriptionCreateDto)
        {
            var subscription = _context.WebhookSubscriptions.FirstOrDefault(s => s.WebhookUri == webhookSubscriptionCreateDto.WebHookURI);

            if (subscription == null)
            {
                subscription = _mapper.Map<WebhookSubscription>(webhookSubscriptionCreateDto);
                subscription.Secret = Guid.NewGuid();
                subscription.WebhookPublisher = "PanAus";
                try
                {
                    _context.WebhookSubscriptions.Add(subscription);
                    _context.SaveChanges();
                }
                catch (Exception exception)
                {
                    return BadRequest(exception.Message);
                }
                var webhookSubscriptionReadDto = _mapper.Map<WebhookSubscriptionReadDto>(subscription);
                return CreatedAtRoute(nameof(GetSubscriptionBySecret), new { secret = webhookSubscriptionReadDto.Secret }, webhookSubscriptionReadDto);
            }
            else
                return NoContent();
        }
    }
}

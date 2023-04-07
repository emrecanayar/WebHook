using Airline.WebAPI.Contexts;
using Airline.WebAPI.Dtos;
using Airline.WebAPI.Entities;
using Airline.WebAPI.MessageBus;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;

namespace Airline.WebAPI.Controllers
{
    //Uçuş ile ilgili işlemlerin bulunduğu controller
    [Route("api/[controller]")]
    [ApiController]
    public class FlightsController : ControllerBase
    {
        private readonly AirlineDbContext _context;
        private readonly IMessageBus _messageBus;
        private readonly IMapper _mapper;

        public FlightsController(AirlineDbContext context, IMapper mapper, IMessageBus messageBus)
        {
            _context = context;
            _mapper = mapper;
            _messageBus = messageBus;
        }

        //Uçuş koduna göre uçuş bilgilerini getirir.
        [HttpGet("{flightCode}", Name = "GetFlightDetailsByCode")]
        public ActionResult<FlightDetailReadDto> GetFlightDetailsByCode(string flightCode)
        {
            Flight? flight =
                    _context
                    .Flights
                    .FirstOrDefault(f => f.Code == flightCode);
            if (flight == null) NotFound();

            return Ok(_mapper.Map<FlightDetailReadDto>(flight));
        }

        //Yeni bir uçuş oluşturmak için kullanılır.
        [HttpPost]
        public ActionResult<FlightDetailReadDto> CreateFlight(FlightDetailCreateDto flightDetailCreateDto)
        {
            Flight? flight =
                _context
                    .Flights
                    .FirstOrDefault(f =>
                        f.Code == flightDetailCreateDto.Code);
            if (flight == null)
            {
                Flight? flightDetailModel = _mapper.Map<Flight>(flightDetailCreateDto);
                try
                {
                    _context.Flights.Add(flightDetailModel);
                    _context.SaveChanges();
                }
                catch (Exception exception)
                {
                    return BadRequest(exception.Message.ToString());
                }

                FlightDetailReadDto flightDetailReadDto = _mapper.Map<FlightDetailReadDto>(flightDetailModel);
                return CreatedAtRoute(nameof(GetFlightDetailsByCode),
                new { flightCode = flightDetailReadDto.Code },
                flightDetailReadDto);
            }
            else
            {
                return NoContent();
            }
        }

        //Var olan ucuş ile ilgili güncelleme yapmak için kullanılır.
        [HttpPut]
        public ActionResult UpdateFlightDetail(FlightDetailUpdateDto flightDetailUpdateDto)
        {
            Flight? flight =
                _context
                    .Flights
                    .FirstOrDefault(f => f.Id == flightDetailUpdateDto.Id);
            if (flight == null) return NotFound();

            _mapper.Map(flightDetailUpdateDto, flight);
            _context.SaveChanges();
            //Güncellenen fiyat bilgisini subscribe olan uygulamalara publish edelim.
            _messageBus.Publish(new()
            {
                Code = flightDetailUpdateDto.Code,
                NewPrice = flightDetailUpdateDto.Price,
                WebhookType = "PriceChange"
            });
            return NoContent();
        }
    }
}

using Airline.WebAPI.Dtos;
using Airline.WebAPI.Entities;
using AutoMapper;

namespace Airline.WebAPI.Profiles
{
    public class FlightDetailProfile : Profile
    {
        public FlightDetailProfile()
        {
            CreateMap<Flight, FlightDetailCreateDto>().ReverseMap();
            CreateMap<Flight, FlightDetailUpdateDto>().ReverseMap();
            CreateMap<Flight, FlightDetailReadDto>().ReverseMap();
            CreateMap<FlightDetailCreateDto, FlightDetailReadDto>().ReverseMap();
        }
    }
}

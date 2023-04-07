using System.ComponentModel.DataAnnotations;

namespace Airline.WebAPI.Dtos
{
    public class FlightDetailCreateDto
    {
        [Required]
        public string FlightCode { get; set; }

        [Required]
        public decimal Price { get; set; }
    }
}
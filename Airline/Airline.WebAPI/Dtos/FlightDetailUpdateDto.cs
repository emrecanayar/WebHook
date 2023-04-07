using System.ComponentModel.DataAnnotations;

namespace Airline.WebAPI.Dtos
{
    public class FlightDetailUpdateDto
    {
        [Required]
        public int Id { get; set; }

        [Required]
        public string Code { get; set; }

        [Required]
        public decimal Price { get; set; }
    }
}

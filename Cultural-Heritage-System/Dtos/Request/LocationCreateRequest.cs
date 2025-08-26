using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Cultural_Heritage_System.Dtos.Request
{
    public class LocationCreateRequest
    {
        public string? Province { get; set; }
        public string? District { get; set; }
        public string? Ward { get; set; }
        public string? AddressDetail { get; set; }
        public decimal Latitude { get; set; }
        public decimal Longitude { get; set; }
    }

}

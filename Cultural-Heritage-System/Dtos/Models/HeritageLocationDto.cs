using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using Cultural_Heritage_System.Common;
using System.Text.Json.Serialization;

namespace Cultural_Heritage_System.Models
{
    public class HeritageLocationDto
    {
        public int Id { get; set; }    
        public string? Province { get; set; }
        public string? District { get; set; }       
        public string? Ward { get; set; }     
        public string? AddressDetail { get; set; }
        public decimal Latitude { get; set; }
        public decimal Longitude { get; set; }
    }
}

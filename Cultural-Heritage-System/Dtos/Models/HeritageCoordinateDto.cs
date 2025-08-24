using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using Cultural_Heritage_System.Common;
using System.Text.Json.Serialization;

namespace Cultural_Heritage_System.Models
{
    public class HeritageCoordinateDto
    {
        public long Id { get; set; }     
        public decimal Latitude { get; set; }
        public decimal Longitude { get; set; }

    }
}

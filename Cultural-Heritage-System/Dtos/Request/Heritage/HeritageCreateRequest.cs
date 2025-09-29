using Cultural_Heritage_System.Dtos.Models;
using Cultural_Heritage_System.Dtos.Request.Location;
using Cultural_Heritage_System.Dtos.Request.Media;
using Cultural_Heritage_System.Dtos.Request.Occurrence;
using System.ComponentModel.DataAnnotations;

namespace Cultural_Heritage_System.Dtos.Request.Heritage
{
    public class HeritageCreateRequest
    {      
        public string Name { get; set; }

        public HeritageDescriptionRequest Description { get; set; }

        public int CategoryId { get; set; }       
    
        public List<MediaRequest> Media { get; set; } = new List<MediaRequest>();
        public List<int>? TagIds { get; set; }
        public List<LocationRequest>? Locations { get; set; } = new List<LocationRequest>();
        public List<OccurrenceRequest> Occurrences { get; set; } = new List<OccurrenceRequest>();
    }

}

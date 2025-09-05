using Cultural_Heritage_System.Dtos.Request.Location;
using Cultural_Heritage_System.Dtos.Request.Media;
using Cultural_Heritage_System.Dtos.Request.Occurrence;
using System.ComponentModel.DataAnnotations;

namespace Cultural_Heritage_System.Dtos.Request.Heritage
{
    public class HeritageUpdateRequest
    {
        [Required]
        public string Name { get; set; }

        public string Description { get; set; }

        [Required]
        public int CategoryId { get; set; }

        public string MapUrl { get; set; }

        public bool IsFeatured { get; set; }

        public List<MediaRequest> Media { get; set; } = new();

        public List<int> TagIds { get; set; } = new();

        public List<LocationRequest> Locations { get; set; } = new();

        public List<OccurrenceRequest> Occurrences { get; set; } = new();
    }


}

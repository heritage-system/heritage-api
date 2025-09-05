using Cultural_Heritage_System.Dtos.Request.Location;
using Cultural_Heritage_System.Dtos.Request.Media;
using Cultural_Heritage_System.Dtos.Request.Occurrence;
using System.ComponentModel.DataAnnotations;

namespace Cultural_Heritage_System.Dtos.Request.Heritage
{
    public class HeritageCreateRequest
    {
        [Required]
        public string Name { get; set; }

        public string Description { get; set; }

        [Required]
        public int CategoryId { get; set; }

        public string MapUrl { get; set; }

        public bool IsFeatured { get; set; }

        // Thêm danh sách các entity liên quan
        public List<MediaRequest> Media { get; set; } = new();
        public List<int>? TagIds { get; set; }
        public List<LocationRequest>? Locations { get; set; }
        public List<OccurrenceRequest> Occurrences { get; set; } = new();
    }

}

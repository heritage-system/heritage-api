using System.ComponentModel.DataAnnotations;

namespace Cultural_Heritage_System.Dtos.Request
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
        public List<MediaCreateRequest> Media { get; set; } = new();
        public List<int>? TagIds { get; set; }
        public List<LocationCreateRequest>? Locations { get; set; }
        public List<OccurrenceCreateRequest> Occurrences { get; set; } = new();
    }

}

using System.ComponentModel.DataAnnotations;

namespace Cultural_Heritage_System.Dtos.Request
{
    public class HeritageRequest
    {
        [Required]
        public string Name { get; set; }

        public string Description { get; set; }

        [Required]
        public int CategoryId { get; set; }

        public string MapUrl { get; set; }

        public bool IsFeatured { get; set; }

        public List<long>? MediaIds { get; set; } 
        public List<int>? TagIds { get; set; }
        public List<int>? LocationIds { get; set; }
    }
}

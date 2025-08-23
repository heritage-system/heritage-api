using System.ComponentModel.DataAnnotations;

namespace Cultural_Heritage_System.Dtos.Request
{
    public class HeritageUpdateRequest
    {
        [Required]
        public long Id { get; set; }

        [Required]
        public string Name { get; set; }

        public string Description { get; set; }

        [Required]
        public int CategoryId { get; set; }

        public string MapUrl { get; set; }

        public bool IsFeatured { get; set; }
    }

}

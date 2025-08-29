using System.ComponentModel.DataAnnotations;

namespace Cultural_Heritage_System.Dtos.Request
{
    public class MediaCreateRequest
    {
        [Required]
        public IFormFile File { get; set; }


        [Required]
        public string Type { get; set; } // IMAGE, VIDEO, DOCUMENT

        public string? Description { get; set; }
    }

}

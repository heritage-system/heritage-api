using System.ComponentModel.DataAnnotations;

namespace Cultural_Heritage_System.Dtos.Request.Media
{
    public class MediaRequest
    {
        [Required]
        public IFormFile File { get; set; }


        [Required]
        public string Type { get; set; } // IMAGE, VIDEO, DOCUMENT

        public string? Description { get; set; }
    }

}

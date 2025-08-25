using System.ComponentModel.DataAnnotations;

namespace Cultural_Heritage_System.Dtos.Request
{
    public class MediaCreateRequest
    {
        public string Type { get; set; } 
        public IFormFile File { get; set; } 
    }

}

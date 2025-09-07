using Microsoft.AspNetCore.Mvc;

namespace Cultural_Heritage_System.Dtos.Request.Media
{

    public class MediaRequest
    {
        [FromForm(Name = "File")]   // 👈 clarify binding name
        public IFormFile? File { get; set; }

        [FromForm(Name = "Type")]
        public string? Type { get; set; }

        [FromForm(Name = "Description")]
        public string? Description { get; set; }
    }


}

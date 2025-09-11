using Cultural_Heritage_System.Common;
using Microsoft.AspNetCore.Mvc;

namespace Cultural_Heritage_System.Dtos.Request.Review
{
    public class ReviewCreateRequest
    {


        [FromForm]
        public int HeritageId { get; set; }

        [FromForm]
        public string Comment { get; set; }

        [FromForm]
        public int? ParentReviewId { get; set; }

        [FromForm]
        public List<ReviewMediaRequest>? Media { get; set; }

    }
    public class ReviewMediaRequest
    {
        [FromForm(Name = "File")]   // 👈 clarify binding name
        public IFormFile? File { get; set; }

        [FromForm(Name = "Type")]
        //Enum 
        public MediaType Type { get; set; }
    }
}

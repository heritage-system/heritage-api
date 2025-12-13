using Cultural_Heritage_System.Common;
using Microsoft.AspNetCore.Mvc;

namespace Cultural_Heritage_System.Dtos.Request.Review
{
    public class ReviewCreateRequest
    {     
        public int HeritageId { get; set; }
        public string Comment { get; set; }

        public int? ParentReviewId { get; set; }

        public List<ReviewMediaRequest>? Media { get; set; }

    }
    public class ReviewMediaRequest
    {       
        public string Url { get; set; }
        public MediaType Type { get; set; }
    }
}

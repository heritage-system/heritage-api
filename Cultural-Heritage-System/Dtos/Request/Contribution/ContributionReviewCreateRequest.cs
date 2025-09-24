using Cultural_Heritage_System.Common;
using Microsoft.AspNetCore.Mvc;

namespace Cultural_Heritage_System.Dtos.Request.Review
{
    public class ContributionReviewCreateRequest
    {
     
        public int ContributionId { get; set; }

     
        public string Comment { get; set; }

      
        public int? ParentReviewId { get; set; }

        

    }  
}

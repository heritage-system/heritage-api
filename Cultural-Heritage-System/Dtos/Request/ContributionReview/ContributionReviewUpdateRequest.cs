using Cultural_Heritage_System.Dtos.Request.Review;

namespace Cultural_Heritage_System.Dtos.Request
{
    public class ContributionReviewUpdateRequest
    {
        public long Id { get; set; }  
        public string Comment { get; set; } = string.Empty;    
    }

}

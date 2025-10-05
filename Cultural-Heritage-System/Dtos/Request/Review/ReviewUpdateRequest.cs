using Cultural_Heritage_System.Dtos.Request.Review;

namespace Cultural_Heritage_System.Dtos.Request
{
    public class ReviewUpdateRequest
    {
        public int Id { get; set; }  // Review to update
        public string Comment { get; set; } = string.Empty;
        //public List<ReviewMediaRequest>? Media { get; set; } // optional media updates
    }

}

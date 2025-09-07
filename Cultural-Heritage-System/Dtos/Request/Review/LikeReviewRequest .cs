namespace Cultural_Heritage_System.Dtos.Request.Review
{
    public class LikeReviewRequest
    {
        public long ReviewId { get; set; }
        public bool Like { get; set; } // true = like, false = unlike
    }

}

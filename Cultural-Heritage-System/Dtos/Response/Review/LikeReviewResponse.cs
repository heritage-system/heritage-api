namespace Cultural_Heritage_System.Dtos.Response.Review
{
    public class  LikeReviewResponse
    {
        public long ReviewId { get; set; }
        public int LikeCount { get; set; }
        public bool LikedByMe { get; set; }
    }

}

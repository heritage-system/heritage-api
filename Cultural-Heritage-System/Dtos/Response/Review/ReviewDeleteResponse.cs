namespace Cultural_Heritage_System.Dtos.Response.Review
{
    public class ReviewDeleteResponse
    {
        public long ReviewId { get; set; }    // Id of deleted review
        public Boolean success { get; set; }
        public string message { get; set; }
    }
}

namespace Cultural_Heritage_System.Dtos.Response.Review
{
    public class ReviewUpdateResponse
    {
        public int Id { get; set; }
        public string Comment { get; set; } = string.Empty;
        public List<ReviewMediaResponse>? Media { get; set; }
    }

}

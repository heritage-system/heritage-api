namespace Cultural_Heritage_System.Dtos.Response.Media
{
    public class MediaResponse
    {
        public long Id { get; set; }
        public string Url { get; set; }
        public string MediaType { get; set; } 
        public string? Description { get; set; }
    }

}

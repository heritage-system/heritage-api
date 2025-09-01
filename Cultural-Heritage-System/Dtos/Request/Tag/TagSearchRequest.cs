namespace Cultural_Heritage_System.Dtos.Request.Tag
{
    public class TagSearchRequest
    {
        public string? Keyword { get; set; }

        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 10;
    }
}
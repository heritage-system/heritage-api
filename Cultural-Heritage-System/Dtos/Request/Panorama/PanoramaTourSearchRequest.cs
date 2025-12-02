using Cultural_Heritage_System.Common;

namespace Cultural_Heritage_System.Dtos.Request.Contributor
{
    public class PanoramaTourSearchRequest
    {
        public string? Keyword { get; set; }
        public List<int>? CategoryIds { get; set; }   
        public SortBy SortBy { get; set; } = SortBy.DATEDESC;
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 5;
    }
}

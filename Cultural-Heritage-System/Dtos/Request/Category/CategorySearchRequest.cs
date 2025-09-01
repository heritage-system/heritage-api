using Cultural_Heritage_System.Common;

namespace Cultural_Heritage_System.Dtos.Request.Category
{
    public class CategorySearchRequest
    {
        public string? Keyword { get; set; }

        public SortBy? SortBy { get; set; }

        public int Page { get; set; } = 1;

        public int PageSize { get; set; } = 5;
    }
}

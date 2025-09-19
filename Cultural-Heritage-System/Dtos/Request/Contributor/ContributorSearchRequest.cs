using Cultural_Heritage_System.Common;

namespace Cultural_Heritage_System.Dtos.Request.Contributor
{
    public class ContributorSearchRequest
    {
        public string? Keyword { get; set; }
        public ContributorStatus? Status { get; set; }
        public SortBy SortBy { get; set; } = SortBy.IDASC;
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 5;
    }
}

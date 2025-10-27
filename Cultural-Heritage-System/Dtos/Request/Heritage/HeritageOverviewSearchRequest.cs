using Cultural_Heritage_System.Common;
using System.ComponentModel.DataAnnotations;

namespace Cultural_Heritage_System.Dtos.Request.Heritage
{
    public class HeritageOverviewSearchRequest
    {
        public int Page { get; set; }

        public int PageSize { get; set; }

        public string? Keyword { get; set; }

        public int? CategoryId { get; set; }

        public List<int>? TagIds { get; set; }
        public SortBy? SortBy { get; set; }
    }
}

using Cultural_Heritage_System.Common;
using System.ComponentModel.DataAnnotations.Schema;

namespace Cultural_Heritage_System.Dtos.Request.Heritage
{
    public class ContributionOverviewSearchRequest
    {
        public string? Keyword { get; set; }      
        public ContributionStatus? ContributionStatus { get; set; } 
        public SortBy? SortBy { get; set; }   
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 5;

    }
}

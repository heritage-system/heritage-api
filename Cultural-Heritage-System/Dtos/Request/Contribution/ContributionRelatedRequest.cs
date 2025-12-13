using Cultural_Heritage_System.Common;
using System.ComponentModel.DataAnnotations.Schema;

namespace Cultural_Heritage_System.Dtos.Request.Heritage
{
    public class ContributionRelatedRequest
    {
        public string? Keyword { get; set; }
        public List<int>? ContributorIds { get; set; }
        public List<int>? TagHeritageIds { get; set; }
        public int? contributionId { get; set; }
        public int Quantity { get; set; } = 6;

    }
}

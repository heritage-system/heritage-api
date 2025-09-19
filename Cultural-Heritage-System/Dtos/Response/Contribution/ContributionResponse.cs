using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using Cultural_Heritage_System.Common;
using Cultural_Heritage_System.Dtos.Response.Heritage;
using Cultural_Heritage_System.Dtos.Models;

namespace Cultural_Heritage_System.Models
{
    public class ContributionResponse
    {
        public int Id { get; set; }
        public int ContributorId { get; set; }
        public string ContributorName { get; set; }
        public string AvatarUrl { get; set; }
        public string Title { get; set; }
        public string MediaUrl { get; set; }
        public string? Content { get; set; }
        public string? PreviewContent { get; set; }
        public DateTimeOffset PublishedAt { get; set; }
        public List<HeritageNameSearchResponse> ContributionHeritageTags { get; set; } = new List<HeritageNameSearchResponse>();
        public string Status { get; set; }    
        public int View { get; set; }
        public SubscriptionDto? Subscription { get; set; }

        public bool IsSave { get; set; } = false;
    }

}

using Cultural_Heritage_System.Common;
using Cultural_Heritage_System.Dtos.Models;
using Cultural_Heritage_System.Dtos.Response.Heritage;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Cultural_Heritage_System.Models
{
    public class ContributionSearchResponse
    {
        public int Id { get; set; }
        public int ContributorId { get; set; }
        public string ContributorName { get; set; }
        public string AvatarUrl { get; set; }
        public string Title { get; set; }
        public string MediaUrl { get; set; }
        public string? FirstContent { get; set; }    
        public DateTimeOffset PublishedAt { get; set; }
        public List<HeritageNameSearchResponse> ContributionHeritageTags { get; set; } = new List<HeritageNameSearchResponse>();
        public int View { get; set; }
        public int Comments { get; set; }
        public bool IsSave { get; set; } = false;
        public bool IsPremium { get; set; } = false;
    }

}

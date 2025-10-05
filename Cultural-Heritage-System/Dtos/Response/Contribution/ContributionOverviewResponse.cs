using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using Cultural_Heritage_System.Common;
using Cultural_Heritage_System.Dtos.Response.Heritage;
using Cultural_Heritage_System.Dtos.Models;

namespace Cultural_Heritage_System.Models
{
    public class ContributionOverviewResponse
    {
        public int Id { get; set; }         
        public string Title { get; set; }
        public string MediaUrl { get; set; }
        public string? Content { get; set; }  
        public DateTimeOffset PublishedAt { get; set; }
        public DateTime CreatedAt { get; set; }
        public List<HeritageNameSearchResponse> ContributionHeritageTags { get; set; } = new List<HeritageNameSearchResponse>();
        public string Status { get; set; }    
        public int View { get; set; }
        public int Save { get; set; }
        public int Comments { get; set; }
        public int Reports { get; set; }
        public bool IsPremium { get; set; } = false;
        public List<MonthlyViewStat> MonthlyViews { get; set; } = new();
    }

}

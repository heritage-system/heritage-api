using Cultural_Heritage_System.Common;
using Cultural_Heritage_System.Dtos.Models;
using Cultural_Heritage_System.Dtos.Response.Heritage;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Cultural_Heritage_System.Models
{
    public class ContributionOverviewListItemResponse
    {
        public int Id { get; set; }     
        public string Title { get; set; }
        public string MediaUrl { get; set; }      
        public List<HeritageNameSearchResponse> ContributionHeritageTags { get; set; } = new List<HeritageNameSearchResponse>();
        public int View { get; set; }
        public int Comments { get; set; }
        public int Saves { get; set; }
        public bool IsPremium { get; set; } = false;
        public string Status { get; set; }
    }

}

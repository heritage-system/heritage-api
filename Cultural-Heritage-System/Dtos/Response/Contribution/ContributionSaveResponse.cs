using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using Cultural_Heritage_System.Common;
using Cultural_Heritage_System.Dtos.Response.Heritage;
using Cultural_Heritage_System.Dtos.Models;

namespace Cultural_Heritage_System.Models
{
    public class ContributionSaveResponse
    {
        public long Id { get; set; }
        public int ContributionId { get; set; }
        public int ContributorId { get; set; }
        public string ContributorName { get; set; }
        public string AvatarUrl { get; set; }
        public string Title { get; set; }
        public string MediaUrl { get; set; }     
        public List<HeritageNameSearchResponse> ContributionHeritageTags { get; set; } = new List<HeritageNameSearchResponse>();       
    }

}

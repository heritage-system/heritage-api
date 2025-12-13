using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using Cultural_Heritage_System.Common;
using Cultural_Heritage_System.Dtos.Response.Heritage;
using Cultural_Heritage_System.Dtos.Models;

namespace Cultural_Heritage_System.Models
{
    public class ContributionDetailUpdatedResponse
    {
        public int Id { get; set; }         
        public string Title { get; set; }
        public string MediaUrl { get; set; }
        public string? Content { get; set; }       
        public List<HeritageNameSearchResponse> ContributionHeritageTags { get; set; } = new List<HeritageNameSearchResponse>();
        public string Status { get; set; }          
        public bool IsPremium { get; set; } = false;      
    }

}

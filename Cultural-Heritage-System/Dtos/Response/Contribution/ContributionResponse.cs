using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using Cultural_Heritage_System.Common;

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
        public string Content { get; set; }
        public decimal Price { get; set; }     
        public string Status { get; set; }    
    }

}

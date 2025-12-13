using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using Cultural_Heritage_System.Common;

namespace Cultural_Heritage_System.Models
{
    public class ContributionUpdateRequest
    {    
        public int Id { get; set; }
        public string Title { get; set; }
        public string Content { get; set; }
        public string MediaUrl { get; set; }
        public List<int>? TagHeritageIds { get; set; }
        public PremiumType PremiumType { get; set; } = PremiumType.FREE;
    }

}

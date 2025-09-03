using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using Cultural_Heritage_System.Common;

namespace Cultural_Heritage_System.Models
{
    public class ContributionCreationRequest
    {               
        public string Title { get; set; }
        public string Content { get; set; }
        public string MediaUrl { get; set; }
        public decimal Price { get; set; }
    }

}

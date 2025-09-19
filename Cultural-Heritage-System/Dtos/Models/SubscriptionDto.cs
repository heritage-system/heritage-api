using Cultural_Heritage_System.Common;
using Cultural_Heritage_System.Helpers;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Cultural_Heritage_System.Dtos.Models
{
    public class SubscriptionDto
    {      
        public int UserId { get; set; }    
        public int PackageId { get; set; }
        public int MaxOpensPerMonth { get; set; }      
        public int OpensUsed { get; set; } = 0;
        
    }


}

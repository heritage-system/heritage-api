using Cultural_Heritage_System.Common;
using Cultural_Heritage_System.Helpers;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Cultural_Heritage_System.Models
{
    public class ContributionAccessLog : BaseEntity<long>
    {
        [Required]
        [ForeignKey("User")]
        [Column("user_id")]
        public int UserId { get; set; }
        public User? User { get; set; }

        [Required]
        [ForeignKey("Contribution")]
        [Column("contribution_id")]
        public int ContributionId { get; set; }
        public Contribution Contribution { get; set; }

      
        [ForeignKey("Subscription")]
        [Column("subscription_id")]
        public int? SubscriptionId { get; set; } 
        public Subscription? Subscription { get; set; }       
       
        [Column("counted_for_quota")]
        public bool CountedForQuota { get; set; } = false;
    }


}

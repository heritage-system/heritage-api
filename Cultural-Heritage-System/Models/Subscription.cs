using Cultural_Heritage_System.Common;
using Cultural_Heritage_System.Helpers;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Cultural_Heritage_System.Models
{
    public class Subscription : BaseEntity<int>
    {
        [Required]
        [ForeignKey("User")]
        [Column("user_id")]
        public int UserId { get; set; }
        public User? User { get; set; }

        [Required]
        [ForeignKey("PremiumPackage")]
        [Column("package_id")]
        public int PackageId { get; set; }
        public PremiumPackage Package { get; set; }

        [Required]
        [Column("start_at")]
        public DateTime StartAt { get; set; }
        [Required]
        [Column("end_at")]
        public DateTime EndAt { get; set; }
       
        //[Column("opens_used")]
        //public int OpensUsed { get; set; } = 0;
        [Required]
        [Column("status", TypeName = "nvarchar(20)")]
        public SubscriptionStatus Status { get; set; } = SubscriptionStatus.ACTIVE;

        public ICollection<SubscriptionUsage> UsageRecords { get; set; } = new List<SubscriptionUsage>();

    }


}

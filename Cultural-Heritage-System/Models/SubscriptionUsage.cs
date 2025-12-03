using Cultural_Heritage_System.Common;
using Cultural_Heritage_System.Helpers;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Cultural_Heritage_System.Models
{
    public class SubscriptionUsage : BaseEntity<long>
    {
        [Required]
        [ForeignKey("Subscription")]
        [Column("subscription_id")]
        public int SubscriptionId { get; set; }
        public Subscription Subscription { get; set; }

        [Column("benefit_name", TypeName = "nvarchar(20)")]
        public BenefitName? BenefitName { get; set; }

        [Column("total")]       // Tổng lượt được cấp khi mua gói
        public int? Total { get; set; }        

        [Column("used")]      // Lượt đã sử dụng
        public int Used { get; set; }
       
        [Column("status", TypeName = "nvarchar(20)")]
        public SubscriptionStatus Status { get; set; } = SubscriptionStatus.ACTIVE;


        // Nếu BenefitType = FeatureUnlock → Total = null (vô hạn)
    }


}

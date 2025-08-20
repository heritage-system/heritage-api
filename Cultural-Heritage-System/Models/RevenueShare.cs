using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using Cultural_Heritage_System.Common;

namespace Cultural_Heritage_System.Models
{
    public class RevenueShare : BaseEntity<int>
    {

        [Required]
        [ForeignKey("Contribution")]
        [Column("contribution_id")]
        public int ContributionId { get; set; }
        public Contribution Contribution { get; set; }

        [Required]
        [ForeignKey("Contributor")]
        [Column("contributor_id")]
        public int ContributorId { get; set; }
        public Contributor Contributor { get; set; }

        [Required]
        [ForeignKey("ContributionPurchase")]
        [Column("purchase_id")]
        public int PurchaseId { get; set; }
        public ContributionPurchase ContributionPurchase { get; set; }

        [Column("total_amount", TypeName = "decimal(10,2)")]
        public decimal TotalAmount { get; set; }

        [Column("contributor_amount", TypeName = "decimal(10,2)")]
        public decimal ContributorAmount { get; set; }

        [Column("system_amount", TypeName = "decimal(10,2)")]
        public decimal SystemAmount { get; set; }
     
        [Column("payout_status", TypeName = "nvarchar(20)")]
        public PaymentStatus PayoutStatus { get; set; } = PaymentStatus.PENDING;
    }

}

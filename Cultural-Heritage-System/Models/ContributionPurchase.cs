using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using Cultural_Heritage_System.Common;

namespace Cultural_Heritage_System.Models
{
    public class ContributionPurchase : BaseEntity<int>
    {

        [Required]
        [Column("user_id")]
        [ForeignKey("User")]
        public int UserId { get; set; }
        public User User { get; set; }

        [Required]
        [ForeignKey("Contribution")]
        [Column("contribution_id")]
        public int ContributionId { get; set; }
        public Contribution Contribution { get; set; }

        [Column("amount", TypeName = "decimal(10,2)")]
        public decimal Amount { get; set; }

        [Column("purchased_at")]
        public DateTime PurchasedAt { get; set; } = DateTime.UtcNow;

        [Column("payment_status", TypeName = "nvarchar(20)")]
        public PaymentStatus PaymentStatus { get; set; } = PaymentStatus.PENDING;

        [Column("payment_method", TypeName = "nvarchar(20)")]
        public PaymentMethod PaymentMethod { get; set; } = PaymentMethod.WALLET;

    }

}

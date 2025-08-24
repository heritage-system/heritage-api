using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using Cultural_Heritage_System.Common;

namespace Cultural_Heritage_System.Models
{
    public class WalletTransaction : BaseEntity<int>
    {
        [Required]
        [ForeignKey("Wallet")]
        [Column("wallet_id")]
        public int WalletId { get; set; }
        public Wallet? Wallet { get; set; }

        [Column("type", TypeName = "nvarchar(20)")]
        public WalletTransactionType Type { get; set; }

        [Column("amount", TypeName = "decimal(18,2)")]
        public decimal Amount { get; set; }

        [Column("description", TypeName = "text")]
        public string? Description { get; set; }    

        [ForeignKey("RelatedPurchase")]
        [Column("related_purchase_id")]
        public int? RelatedPurchaseId { get; set; }
        public ContributionPurchase? RelatedPurchase { get; set; }

    }
}

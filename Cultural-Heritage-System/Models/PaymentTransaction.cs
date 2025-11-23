using Cultural_Heritage_System.Common;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Cultural_Heritage_System.Models
{
    public class PaymentTransaction : BaseEntity<long>
    {
        [Required]
        [ForeignKey("User")]
        [Column("user_id")]
        public int UserId { get; set; }
        public User? User { get; set; }

        [Column("type", TypeName = "nvarchar(20)")]
        public WalletTransactionType Type { get; set; }

        [Column("amount", TypeName = "decimal(18,2)")]
        public decimal Amount { get; set; }

        [Column("description", TypeName = "text")]
        public string? Description { get; set; }

    }
}

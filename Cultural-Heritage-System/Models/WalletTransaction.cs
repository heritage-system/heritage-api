using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using Cultural_Heritage_System.Common;

namespace Cultural_Heritage_System.Models
{
    public class WalletTransaction : BaseEntity<int>
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

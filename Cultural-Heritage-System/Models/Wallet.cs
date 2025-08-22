using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using Cultural_Heritage_System.Common;

namespace Cultural_Heritage_System.Models
{
    public class Wallet : BaseEntity<int>
    {
        [Required]
        [ForeignKey("User")]
        [Column("user_id")]
        public int UserId { get; set; }
        public User? User { get; set; }

        [Column("balance", TypeName = "decimal(18,2)")]
        public decimal Balance { get; set; } = 0;

        public ICollection<WalletTransaction> Transactions { get; set; } = new List<WalletTransaction>();

    }
}

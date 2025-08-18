using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using Cultural_Heritage_System.Common;

namespace Cultural_Heritage_System.Models
{
    public class PasswordReset : BaseEntity<int>
    {
        [Required]
        [Column("user_id")]
        [ForeignKey("User")]
        public int UserId { get; set; }
        public User User { get; set; }

      
        [Required]
        [Column("code_hash")]
        public string CodeHash { get; set; }

       
        [Required]
        [Column("expires_at")]
        public DateTime ExpiresAt { get; set; }

       
        [Column("used")]
        public bool Used { get; set; } = false;

      
        [Column("attempts")]
        public int Attempts { get; set; } = 0;

       
        [Column("created_ip")]
        public string? CreatedIp { get; set; }

        [Column("verified_at")]
        public DateTime? VerifiedAt { get; set; }
    }

}

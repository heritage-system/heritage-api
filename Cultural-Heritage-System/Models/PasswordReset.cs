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

        [Column("token")]
        public string Token { get; set; }

        [Column("expires_at")]
        public DateTime ExpiresAt { get; set; }
    }

}

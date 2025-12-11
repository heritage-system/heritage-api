using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using Cultural_Heritage_System.Common;

namespace Cultural_Heritage_System.Models
{
    public class ConfirmToken : BaseEntity<long>
    {
        [Required]
        [Column("user_id")]
        [ForeignKey("User")]
        public int UserId { get; set; }
        public User User { get; set; }

        [Column("expires")]
        public DateTime Expires { get; set; }

        [Required]
        [Column("token")]
        public string Token { get; set; }
     
        [Column("revoked")]
        public DateTime? Revoked { get; set; }

        public bool IsExpired => DateTime.UtcNow >= Expires;
        public bool IsActive => Revoked == null && !IsExpired;

        public ConfirmToken()
        {
            Expires = DateTime.UtcNow.AddDays(15);
        }
    }
}


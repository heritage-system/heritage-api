using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using Cultural_Heritage_System.Common;

namespace Cultural_Heritage_System.Models
{
    public class User : BaseEntity<int>
    {
        [Column("username")]
        public string UserName { get; set; }

        [Column("email")]
        public string Email { get; set; }

        [Column("password_hash")]
        public string PasswordHash { get; set; }

        [Column("full_name")]
        public string FullName { get; set; }

     
        [Column("user_status", TypeName = "nvarchar(20)")]
        public UserStatus UserStatus { get; set; } = UserStatus.ACTIVE;

        [Column("two_factor_secret")]
        public string? TwoFactorSecret { get; set; }

        [Column("is_2fa_verified")]
        public bool Enable2FA { get; set; } = false;


        [Required]
        [ForeignKey("Role")]
        [Column("role_id")]
        public int RoleId { get; set; }
        public Role? Role { get; set; }

        public ICollection<Favorite> Favorites { get; set; } = new List<Favorite>();
        public ICollection<Review> Reviews { get; set; } = new List<Review>();
        public ICollection<Report> Reports { get; set; } = new List<Report>();
        public ICollection<Contributor> Contributors { get; set; } = new List<Contributor>();
        public ICollection<Contribution> ReviewedContributions { get; set; } = new List<Contribution>();

        public ICollection<Wallet> Wallets { get; set; } = new List<Wallet>();
        public ICollection<QuizResult> QuizResults { get; set; } = new List<QuizResult>();
        public ICollection<SystemLog> SystemLogs { get; set; } = new List<SystemLog>();
        public ICollection<Notification> Notifications { get; set; } = new List<Notification>();
    }
}

using Cultural_Heritage_System.Common;
using Cultural_Heritage_System.Helpers;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Cultural_Heritage_System.Models
{
    public class User : BaseEntity<int>, IUnsignedEntity
    {
        [Column("username")]
        public string UserName { get; set; }

        [Column("email")]
        public string Email { get; set; }

        [Column("password_hash")]
        public string PasswordHash { get; set; }

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

        public ICollection<PaymentTransaction> PaymentTransactions { get; set; } = new List<PaymentTransaction>();
        public ICollection<QuizResult> QuizResults { get; set; } = new List<QuizResult>();
        public ICollection<SystemLog> SystemLogs { get; set; } = new List<SystemLog>();
        public ICollection<Notification> Notifications { get; set; } = new List<Notification>();

        public ICollection<ReviewLike> ReviewLikes { get; set; } = new List<ReviewLike>();

        public ICollection<ReviewReport> ReviewReports { get; set; } = new List<ReviewReport>();

        public ICollection<RefreshToken> RefreshTokens { get; set; } = new List<RefreshToken>();
        public ICollection<ConfirmToken> ConfirmTokens { get; set; } = new List<ConfirmToken>();

        public ICollection<Subscription> Subscriptions { get; set; } = new List<Subscription>();

        public ICollection<ContributionAccessLog> ContributionAccessLogs { get; set; } = new List<ContributionAccessLog>();
        public ICollection<ContributionUnlock> ContributionUnlocks { get; set; } = new List<ContributionUnlock>();
        public ICollection<ContributionSave> ContributionSaves { get; set; } = new List<ContributionSave>();

        public ICollection<ContributionReview> ContributionReviews { get; set; } = new List<ContributionReview>();
        public ICollection<ContributionReviewLike> ContributionReviewLike { get; set; } = new List<ContributionReviewLike>();

        public ICollection<PanoramaSceneUnlock> PanoramaSceneUnlocks { get; set; } = new List<PanoramaSceneUnlock>();
        public ICollection<ContributionReport> ContributionReports { get; set; } = new List<ContributionReport>();
        public Contributor? Contributor { get; set; }        
        public Profile? Profile { get; set; }
        public Staff? Staff { get; set; }

        [Column("user_name_unsigned")]
        public string UserNameUnsigned { get; set; }
        public void GenerateUnsignedFields()
        {
            UserNameUnsigned = StringHelper.RemoveDiacritics(UserName).ToLower();
        }
    }
}

using Cultural_Heritage_System.Common;
using Cultural_Heritage_System.Helpers;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Cultural_Heritage_System.Models
{
    public class Contribution : BaseEntity<int>, IUnsignedEntity
    {
        [Required]
        [Column("contributor_id")]
        [ForeignKey(nameof(Contributor))]
        public int ContributorId { get; set; }
        public Contributor Contributor { get; set; }

        [Required, MaxLength(200)]
        [Column("title")]
        public string Title { get; set; }

        [Required]
        [Column("content")]
        public string Content { get; set; }

        // Giá áp dụng khi PremiumType là OneTimePurchaseOnly / Hybrid
        //[Column("price", TypeName = "decimal(18,2)")]
        //[Range(0, double.MaxValue)]
        //public decimal? Price { get; set; }

        //[Column("currency", TypeName = "char(3)")]
        //public string Currency { get; set; } = "VND"; // hoặc "USD"

        [Column("media_url")]
        public string MediaUrl { get; set; }

        [Column("status", TypeName = "nvarchar(30)")]
        public ContributionStatus Status { get; set; } = ContributionStatus.PENDING;

        [Column("premium_type", TypeName = "nvarchar(30)")]
        public PremiumType PremiumType { get; set; } = PremiumType.FREE;

        // Paywall/Preview
        [Column("preview_content")]
        public string? PreviewContent { get; set; }

        [Column("first_content")]
        public string? FirstContent { get; set; }

        //// Thống kê cơ bản
        //[Column("word_count")]
        //public int WordCount { get; set; }
        //[Column("read_time_seconds")]
        //public int EstimatedReadTimeSeconds { get; set; }

        // Duyệt
        [Column("reviewed_by")]
        [ForeignKey(nameof(Reviewer))]
        public int? ReviewedBy { get; set; }
        public User Reviewer { get; set; }

        [Column("approved_at")]
        public DateTimeOffset? ApprovedAt { get; set; }

        [Column("published_at")]
        public DateTimeOffset? PublishedAt { get; set; }

        //// Chia sẻ doanh thu (có thể override theo bài)
        //[Column("revenue_share_rate", TypeName = "decimal(5,2)")] // 70.00 = 70%
        //public decimal? RevenueShareRate { get; set; }

        // SEO / tìm kiếm
        [Column("title_unsigned")]
        public string TitleUnsigned { get; set; }

        public ICollection<ContributionHeritageTag> ContributionHeritageTags { get; set; } = new List<ContributionHeritageTag>();
        public ICollection<ContributionAccessLog> ContributionAccessLogs { get; set; } = new List<ContributionAccessLog>();
        public ICollection<ContributionUnlock> ContributionUnlocks { get; set; } = new List<ContributionUnlock>();
        public ICollection<ContributionSave> ContributionSaves { get; set; } = new List<ContributionSave>();

        public ICollection<ContributionReview> Reviews { get; set; } = new List<ContributionReview>();
        public void GenerateUnsignedFields()
        {
            TitleUnsigned = StringHelper.RemoveDiacritics(Title).ToLowerInvariant();
        }
    }


}

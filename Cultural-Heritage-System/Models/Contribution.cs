using Cultural_Heritage_System.Common;
using Cultural_Heritage_System.Helpers;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Cultural_Heritage_System.Models
{
    public class Contribution: BaseEntity<int>,IUnsignedEntity
    {
        [Required]
        [Column("contributor_id")]
        [ForeignKey(nameof(Contributor))]
        public int ContributorId { get; set; }
        public Contributor Contributor { get; set; }

        [Column("title")]
        public string Title { get; set; }

        [Column("content")]
        public string Content { get; set; }

        [Column("price ", TypeName = "decimal(18,2)")]
        [Range(0, double.MaxValue)]
        public decimal Price { get; set; }

        [Column("media_url")]
        public string MediaUrl { get; set; }

        [Column("status" ,TypeName = "nvarchar(20)")]
        public ContributionStatus Status { get; set; } = ContributionStatus.PENDING;

        [Column("reviewed_by")]
        [ForeignKey(nameof(Reviewer))]
        public int? ReviewedBy { get; set; }
        public User? Reviewer { get; set; }

        [Column("title_unsigned")]
        public string TitleUnsigned { get; set; }
        public void GenerateUnsignedFields()
        {
            TitleUnsigned = StringHelper.RemoveDiacritics(Title).ToLower();
        }

    }

}

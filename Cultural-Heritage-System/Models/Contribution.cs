using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using Cultural_Heritage_System.Common;

namespace Cultural_Heritage_System.Models
{
    public class Contribution: BaseEntity<int>
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

        [Column("status")]
        public string Status { get; set; }

        [Column("reviewed_by")]
        [ForeignKey(nameof(Reviewer))]
        public int ReviewedBy { get; set; }
        public User Reviewer { get; set; }
    }

}

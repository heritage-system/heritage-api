using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using Cultural_Heritage_System.Common;

namespace Cultural_Heritage_System.Models
{
    public class ContributionReview: BaseEntity<long>
    {
        [Required]
        [Column("user_id")]
        [ForeignKey("User")]       
        public int UserId { get; set; }
        public User User { get; set; }

        [Required]
        [Column("contribution_id")]
        [ForeignKey("Contribution")]
        public int ContributionId { get; set; }
        public Contribution Contribution { get; set; }

        [Required]
        [Column("comment")]
        public string Comment { get; set; }

        [Column("parent_review_id")]
        public long? ParentReviewId { get; set; }
        public ContributionReview? ParentReview { get; set; }

        public ICollection<ContributionReview>? Replies { get; set; }
        public ICollection<ContributionReviewLike>? Likes { get; set; } 
      
    }

}

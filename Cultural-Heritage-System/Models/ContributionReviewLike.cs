using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using Cultural_Heritage_System.Common;

namespace Cultural_Heritage_System.Models
{
    public class ContributionReviewLike : BaseEntity<long>
    {
        [Required]
        [ForeignKey("Review")]
        [Column("contribution_review_id")]
        public long ContributionReviewId { get; set; }
        public ContributionReview ContributionReview { get; set; }

        [Required]
        [Column("user_id")]
        [ForeignKey("User")]
        public int UserId { get; set; }
        public User User { get; set; }
      
       
    }

}

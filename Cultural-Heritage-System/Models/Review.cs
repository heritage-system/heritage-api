using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using Cultural_Heritage_System.Common;

namespace Cultural_Heritage_System.Models
{
    public class Review: BaseEntity<long>
    {
        [Required]
        [Column("user_id")]
        [ForeignKey("User")]       
        public int UserId { get; set; }
        public User User { get; set; }

        [Required]
        [Column("heritage_id")]
        [ForeignKey("Heritage")]      
        public long HeritageId { get; set; }
        public Heritage Heritage { get; set; }
  
        [Required]
        [Column("comment")]
        public string Comment { get; set; }

        [Column("parent_review_id")]
        public long? ParentReviewId { get; set; }
        public Review? ParentReview { get; set; }

        public ICollection<Review>? Replies { get; set; }  
        public ICollection<ReviewLike>? Likes { get; set; } 
        public ICollection<ReviewReport>? Reports { get; set; }
        public ICollection<ReviewMedia>? ReviewMedias { get; set; }
    }

}

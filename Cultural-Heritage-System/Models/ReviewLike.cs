using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using Cultural_Heritage_System.Common;

namespace Cultural_Heritage_System.Models
{
    public class ReviewLike : BaseEntity<long>
    {
        [Required]
        [ForeignKey("Review")]
        [Column("review_id")]
        public long ReviewId { get; set; }
        public Review Review { get; set; }

        [Required]
        [Column("user_id")]
        [ForeignKey("User")]
        public int UserId { get; set; }
        public User User { get; set; }
      
       
    }

}

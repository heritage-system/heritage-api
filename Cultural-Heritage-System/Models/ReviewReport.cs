using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using Cultural_Heritage_System.Common;

namespace Cultural_Heritage_System.Models
{
    public class ReviewReport: BaseEntity<long>
    {
        [Required]
        [Column("user_id")]
        [ForeignKey("User")]       
        public int UserId { get; set; }
        public User User { get; set; }

        [Required]
        [Column("review_id")]
        [ForeignKey("Review")]      
        public long ReviewId { get; set; }
        public Review Review { get; set; }

        [Required]
        [Column("reason")]
        public string Reason { get; set; }

    }

}

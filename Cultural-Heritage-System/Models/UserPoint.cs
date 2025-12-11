using Cultural_Heritage_System.Common;
using Cultural_Heritage_System.Helpers;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Cultural_Heritage_System.Models
{
    public class UserPoint : BaseEntity<int>
    {
        [Required]
        [ForeignKey("User")]
        [Column("user_id")]
        public int UserId { get; set; }
        public User? User { get; set; }
          
        [Column("total_points")]
        public int TotalPoints { get; set; } = 0;

        [Column("unlock_tokens")]
        public int UnlockTokens { get; set; } = 0;
       
    }


}

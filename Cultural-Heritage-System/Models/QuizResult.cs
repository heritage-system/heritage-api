using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using Cultural_Heritage_System.Common;

namespace Cultural_Heritage_System.Models
{
    public class QuizResult : BaseEntity<int>
    {

        [Required]
        [Column("user_id")]
        [ForeignKey("User")]
        public int UserId { get; set; }
        public User User { get; set; }

        [Column("score")]
        public int Score { get; set; }

        [Column("quiz_id")]
        [ForeignKey("Quiz")]
        public long QuizId { get; set; }
        public Quiz Quiz { get; set; }

        [Column("completed_at")]
        public DateTime CompletedAt { get; set; }

    }

}

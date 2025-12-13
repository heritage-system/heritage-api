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

        [Column("number_of_clear")]
        public int NumberOfClear { get; set; }

        [Column("quiz_id")]
        [ForeignKey(nameof(Quiz))]
        public long QuizId { get; set; }
        public Quiz Quiz { get; set; }
    }

}

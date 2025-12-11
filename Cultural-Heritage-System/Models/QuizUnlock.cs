using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using Cultural_Heritage_System.Common;

namespace Cultural_Heritage_System.Models
{
    public class QuizUnlock : BaseEntity<long>
    {

        [Required]
        [Column("user_id")]
        [ForeignKey("User")]
        public int UserId { get; set; }
        public User User { get; set; }

        [Required]
        [ForeignKey("Quiz")]
        [Column("quiz_id")]
        public long QuizId { get; set; }
        public Quiz Quiz { get; set; }

        [Column("unlocking_method", TypeName = "nvarchar(30)")]
        public UnlockingMethod UnlockingMethod { get; set; } = UnlockingMethod.BY_SUBSCRIPTION;


    }

}

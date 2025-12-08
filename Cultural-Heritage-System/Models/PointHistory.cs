using Cultural_Heritage_System.Common;
using Cultural_Heritage_System.Helpers;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Cultural_Heritage_System.Models
{
    public class PointHistory: BaseEntity<long>
    {
        [Required]
        [ForeignKey("User")]
        [Column("user_id")]
        public int UserId { get; set; }
        public User? User { get; set; }
          
        [Column("change_amount")]
        public int ChangeAmount { get; set; }

        [Column("old_value")]
        public int OldValue { get; set; }

        [Column("new_value")]
        public int NewValue { get; set; }

        [Column("reason", TypeName = "nvarchar(20)")]
        public PointHistoriesReason Reason { get; set; }

        [Column("reference_id")] //-- matchId, articleId, quizId… nếu cần
        public long ReferenceId { get; set; }

    }


}

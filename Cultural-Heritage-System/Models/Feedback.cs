using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using Cultural_Heritage_System.Common;

namespace Cultural_Heritage_System.Models
{
    public class Feedback: BaseEntity<int>
    {
        [Required]
        [Column("user_id")]
        [ForeignKey("User")]       
        public int UserId { get; set; }
        public User User { get; set; }

        [Column("subject")]
        public string Subject { get; set; }

        [Column("message")]
        public string Message { get; set; }

        [Column("resolved")]
        public bool Resolved { get; set; }

    }

}

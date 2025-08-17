using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using Cultural_Heritage_System.Common;

namespace Cultural_Heritage_System.Models
{
    public class Notification: BaseEntity<int>
    {
        [Required]
        [Column("user_id")]
        [ForeignKey("User")]       
        public int UserId { get; set; }
        public User User { get; set; }

        [Column("title")]
        public string Title { get; set; }

        [Column("message")]
        public string Message { get; set; }

        [Column("type")]
        public string Type { get; set; }

        [Column("is_read")]
        public bool IsRead { get; set; }
    }

}

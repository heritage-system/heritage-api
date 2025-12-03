using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Cultural_Heritage_System.Models
{
    public class EventRegistration : BaseEntity<long>
    {
        [Required]
        [Column("event_id")]
        public long EventId { get; set; }
        public Event Event { get; set; }

        [Required]
        [Column("user_id")]
        public int UserId { get; set; }
        public User User { get; set; }

        [Column("registered_at")]
        public DateTime RegisteredAt { get; set; }

        [Column("is_cancelled")]
        public bool IsCancelled { get; set; }
    }
}

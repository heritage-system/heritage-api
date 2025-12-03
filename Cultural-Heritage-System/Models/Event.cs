using Cultural_Heritage_System.Common;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Cultural_Heritage_System.Models
{
    public class Event : BaseEntity<long>
    {
        [Required]
        [MaxLength(255)]
        [Column("title")]
        public string Title { get; set; } = string.Empty;

        [Column("description")]
        public string? Description { get; set; }

        [Column("thumbnail_url")]
        public string? ThumbnailUrl { get; set; }

        [Required]
        [Column("start_at")]
        public DateTime StartAt { get; set; }

        [Column("close_at")]
        public DateTime? CloseAt { get; set; }

        [Column("status", TypeName = "nvarchar(20)")]
        public EventStatus Status { get; set; }

        [Column("category", TypeName = "nvarchar(30)")]
        public EventCategory Category { get; set; }

        // Lưu dạng int (EF sẽ map enum -> int)
        [Column("tags")]
        public EventTag Tags { get; set; }

        [Required]
        [Column("created_by_user_id")]
        public int CreatedByUserId { get; set; }
        public User CreatedBy { get; set; }

        // 1 Event – N StreamingRoom
        public ICollection<StreamingRoom> StreamingRooms { get; set; } = new List<StreamingRoom>();

        // 1 Event – N registrations
        public ICollection<EventRegistration> Registrations { get; set; } = new List<EventRegistration>();
    }
}

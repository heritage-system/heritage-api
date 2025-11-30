using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Cultural_Heritage_System.Models
{
    public class RoomChatMessage : BaseEntity<long>
    {
        [ForeignKey("Room"), Column("room_id")] public int RoomId { get; set; }
        public StreamingRoom Room { get; set; } = default!;

        [ForeignKey("User"), Column("user_id")] public int? UserId { get; set; }
        public User? User { get; set; }
        [Required, Column("content")] public string Content { get; set; } = default!;
        [Column("is_system")] public bool IsSystem { get; set; } = false;
    }
}

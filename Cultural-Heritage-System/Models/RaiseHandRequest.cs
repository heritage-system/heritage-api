using Cultural_Heritage_System.Common;
using System.ComponentModel.DataAnnotations.Schema;

namespace Cultural_Heritage_System.Models
{
    public class RaiseHandRequest : BaseEntity<int>
    {
        [ForeignKey("Room"), Column("room_id")] public int RoomId { get; set; }
        public StreamingRoom Room { get; set; } = default!;

        [ForeignKey("User"), Column("user_id")] public int UserId { get; set; }
        public User User { get; set; } = default!;

        [Column("status")] public RaiseHandStatus Status { get; set; } = RaiseHandStatus.Pending;
        [Column("note")] public string? Note { get; set; }
        [Column("resolved_at")] public DateTime? ResolvedAt { get; set; }
    }
}

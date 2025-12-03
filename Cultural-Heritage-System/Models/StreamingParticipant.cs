using Cultural_Heritage_System.Common;
using System.ComponentModel.DataAnnotations.Schema;

namespace Cultural_Heritage_System.Models
{
    public class StreamingParticipant : BaseEntity<int>
    {
        [ForeignKey("Room"), Column("room_id")]
        public int RoomId { get; set; }
        public StreamingRoom Room { get; set; } = default!;

        [ForeignKey("User"), Column("user_id")]
        public int UserId { get; set; }
        public User User { get; set; } = default!;

        [Column("role")]
        public RoomRole Role { get; set; } = RoomRole.AUDIENCE;

        [Column("status")]
        public ParticipantStatus Status { get; set; } = ParticipantStatus.WAITING;

        // ❌ BỎ giơ tay
        // [Column("is_raised_hand")] public bool IsRaisedHand { get; set; } = false;

        // map tới UID/Identity của client dùng cho SDK
        [Column("rtc_uid")]
        public string RtcUid { get; set; } = default!;

        [Column("last_seen_at")]
        public DateTime? LastSeenAt { get; set; }

        [Column("left_at")]
        public DateTime? LeftAt { get; set; }
    }
}

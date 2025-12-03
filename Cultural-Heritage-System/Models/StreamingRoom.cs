using Cultural_Heritage_System.Common;
using Cultural_Heritage_System.Helpers;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Cultural_Heritage_System.Models


{
    public class StreamingRoom : BaseEntity<int>, IUnsignedEntity
    {
        [Required, Column("room_name")]
        public string RoomName { get; set; } = default!;

        [Column("title")]
        public string? Title { get; set; }

        [Column("description")]
        public string? Description { get; set; }

        [Column("max_participants")]
        public int MaxParticipants { get; set; } = 1000;

        [Column("is_active")]
        public bool IsActive { get; set; } = true;

        // 🔥 NEW – thời điểm bắt đầu sự kiện/stream
        [Column("start_at")]
        public DateTime StartAt { get; set; }
        public DateTime? ClosedAt { get; set; }
        [ForeignKey("CreatedBy"), Column("created_by")]
        public int CreatedByUserId { get; set; }
        public User CreatedBy { get; set; } = default!;
        public StreamingRoomType Type { get; set; } = StreamingRoomType.UPCOMING;
        public ICollection<StreamingParticipant> Participants { get; set; }
            = new List<StreamingParticipant>();
        [Column("title_unsigned")] public string? TitleUnsigned { get; set; }
        [Column("event_id")]
        public long? EventId { get; set; }
        public Event? Event { get; set; }
        public void GenerateUnsignedFields()
        {
            TitleUnsigned = StringHelper.RemoveDiacritics(Title).ToLower();
        }
        // ❌ ĐÃ BỎ: RaiseHands, ChatMessages
        // public ICollection<RaiseHandRequest> RaiseHands { get; set; }
        // public ICollection<RoomChatMessage> ChatMessages { get; set; }
    }





}

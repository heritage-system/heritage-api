using Cultural_Heritage_System.Common;

namespace Cultural_Heritage_System.Dtos.Response.Streaming
{
    public class StreamingRoomDetailResponse : StreamingRoomResponse
    {
        public List<StreamingParticipantResponse> Participants { get; set; } = new();
    }
    public class StreamingRoomResponse
    {
        public int Id { get; set; }
        public string RoomName { get; set; } = null!;
        public string? Title { get; set; }

        public bool IsActive { get; set; }
        public int CreatedByUserId { get; set; }
        public DateTime CreatedAt { get; set; }

        // 🔥 NEW
        public DateTime StartAt { get; set; }
        public StreamingRoomType Type { get; set; }
        public DateTime? ClosedAt { get; set; }

        // 🔥 NEW: EventId của sự kiện gắn với room này
        public long? EventId { get; set; }
    }
    public class StreamingRequestJoinResponse
    {
        public string? RtcUid { get; set; }
        public string? RoomName { get; set; }
    }

    public class StreamingParticipantResponse
    {
        public int Id { get; set; }
        public int RoomId { get; set; }
        public int UserId { get; set; }
        public RoomRole Role { get; set; }
        public ParticipantStatus Status { get; set; }
        public string RtcUid { get; set; } = default!;
        public DateTime CreatedAt { get; set; }
    }
    public class StreamingRoomWithCountResponse
    {
        public int Id { get; set; }
        public string RoomName { get; set; } = "";
        public string Title { get; set; } = "";
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }

        public int ParticipantCount { get; set; } // số người theo status yêu cầu (mặc định Admitted)
    }

}

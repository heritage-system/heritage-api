using Cultural_Heritage_System.Common;

namespace Cultural_Heritage_System.Dtos.Response.Event
{
    public class EventResponse
    {
        public long Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string? Description { get; set; }
        public string? ThumbnailUrl { get; set; }
        public DateTime StartAt { get; set; }
        public DateTime? CloseAt { get; set; }
        public EventStatus Status { get; set; }
        public EventCategory Category { get; set; }
        public EventTag Tags { get; set; }

        public int RegisteredCount { get; set; }
        public bool RegisteredByMe { get; set; }

        public int CreatedByUserId { get; set; }
        public string? CreatedByUserName { get; set; }

        public List<StreamingRoomSummaryResponse> StreamingRooms { get; set; } = new();
    }

    // Tóm tắt room trong event detail
    public class StreamingRoomSummaryResponse
    {
        public int Id { get; set; }
        public string RoomName { get; set; }
        public string? Title { get; set; }
        public DateTime? StartAt { get; set; }
        public StreamingRoomType Type { get; set; }
        public bool IsActive { get; set; }
    }

    public class EventRegistrationResponse
    {
        public long EventId { get; set; }
        public bool Registered { get; set; }
    }
    public class EventRegistrationUserResponse
    {
        public long EventId { get; set; }

        public int UserId { get; set; }
        public string UserName { get; set; } = null!;
        public string Email { get; set; } = null!;

        public DateTime RegisteredAt { get; set; }
    }
}

using Cultural_Heritage_System.Common;

namespace Cultural_Heritage_System.Dtos.Request.Event
{
    public class EventCreateRequest
    {
        public string Title { get; set; } = string.Empty;
        public string? Description { get; set; }
        public string? ThumbnailUrl { get; set; } // FE upload lên /file/upload/image trước
        public DateTime StartAt { get; set; }
        public DateTime? CloseAt { get; set; }
        public EventCategory Category { get; set; }
        public EventTag Tags { get; set; } = EventTag.NONE;
    }

    public class EventUpdateRequest
    {
        public long Id { get; set; }              // sẽ gán từ route
        public string? Title { get; set; }
        public string? Description { get; set; }
        public string? ThumbnailUrl { get; set; }
        public DateTime? StartAt { get; set; }
        public DateTime? CloseAt { get; set; }
        public EventStatus? Status { get; set; }
        public EventCategory? Category { get; set; }
        public EventTag? Tags { get; set; }
    }
}

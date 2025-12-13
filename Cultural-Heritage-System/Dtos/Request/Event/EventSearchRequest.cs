// Dtos/Request/Event/EventSearchRequest.cs
using Cultural_Heritage_System.Common;

namespace Cultural_Heritage_System.Dtos.Request.Event
{
    public class EventSearchRequest
    {
        public string? Keyword { get; set; }

        public EventCategory? Category { get; set; }

        // EventTag là [Flags]
        public EventTag? Tag { get; set; }

        // UPCOMING / LIVE / CLOSED / ...
        public EventStatus? Status { get; set; }

        // Lọc theo thời gian bắt đầu event
        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }

        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 10;
    }
}

using Cultural_Heritage_System.Common;

namespace Cultural_Heritage_System.Dtos.Request.Event
{
    // CREATE: Event + many rooms
    public class EventWithRoomsCreateRequest
    {
        // ---- Event fields ----
        public string Title { get; set; } = null!;
        public string? Description { get; set; }
        public string? ThumbnailUrl { get; set; }
        public DateTime StartAt { get; set; }
        public DateTime? CloseAt { get; set; }
        public EventCategory Category { get; set; }
        public EventTag Tags { get; set; }

        /// <summary>
        /// Danh sách room của event. Có thể rỗng.
        /// </summary>
        public List<StreamingRoomForEventCreateRequest> Rooms { get; set; } = new();
    }

    public class StreamingRoomForEventCreateRequest
    {
        // Nếu rỗng -> dùng Event.Title
        public string? Title { get; set; }

        // Nếu null -> dùng Event.StartAt
        public DateTime? StartAt { get; set; }

        // Mặc định UPCOMING
        public StreamingRoomType? Type { get; set; }
    }

    // UPDATE: Event + many rooms (upsert)
    public class EventWithRoomsUpdateRequest
    {
        public long Id { get; set; }

        // ---- Event fields (nullable = optional) ----
        public string? Title { get; set; }
        public string? Description { get; set; }
        public string? ThumbnailUrl { get; set; }
        public DateTime? StartAt { get; set; }
        public DateTime? CloseAt { get; set; }
        public EventStatus? Status { get; set; }
        public EventCategory? Category { get; set; }
        public EventTag? Tags { get; set; }

        /// <summary>
        /// Danh sách room để update / tạo thêm.
        /// - Nếu Id > 0: update room đó
        /// - Nếu Id = null hoặc 0: tạo room mới
        /// - Room đang tồn tại nhưng không nằm trong danh sách này: giữ nguyên (không xoá).
        /// </summary>
        public List<StreamingRoomForEventUpdateRequest> Rooms { get; set; } = new();
    }

    public class StreamingRoomForEventUpdateRequest
    {
        public int? Id { get; set; }          // null/0 → new room
        public string? Title { get; set; }
        public DateTime? StartAt { get; set; }
        public StreamingRoomType? Type { get; set; }
    }
}

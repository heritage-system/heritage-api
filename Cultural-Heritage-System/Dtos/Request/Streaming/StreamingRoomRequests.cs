using Cultural_Heritage_System.Common;
using System.ComponentModel.DataAnnotations;

namespace Cultural_Heritage_System.Dtos.Request.Streaming
{
    public class StreamingRoomCreateRequest
    {
        public string Title { get; set; } = default!;
        public DateTime StartAt { get; set; }
        public long? EventId { get; set; } // NEW// ✨
    }

    public class StreamingRoomUpsertDto
    {
        public long? Id { get; set; } // null => room mới, có Id => update

        [Required]
        public string Title { get; set; } = default!;

        public DateTime? StartAt { get; set; }

        // cho phép client set, hoặc luôn UPCOMING tuỳ bạn
        public StreamingRoomType Type { get; set; } = StreamingRoomType.UPCOMING;
    }
    // ❌ KHÔNG CẦN StreamingRequestJoinRequest nữa
    // public class StreamingRequestJoinRequest { ... }

    public class StreamingAdmitRejectRequest
    {
        [Required] public int UserId { get; set; }
    }

    public class StreamingSetRoleRequest
    {
        [Required] public int UserId { get; set; }
        [Required] public RoomRole Role { get; set; }
    }
    public class StreamingRoomUpdateRequest
    {
        public string? Title { get; set; }
        public DateTime? StartAt { get; set; }
        public StreamingRoomType? Type { get; set; }
    }
    // ❌ KHÔNG CẦN StreamingRaiseHandRequest nữa
}

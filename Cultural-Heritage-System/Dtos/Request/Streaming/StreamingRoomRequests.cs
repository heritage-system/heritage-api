// Dtos/Request/Streaming/StreamingRequests.cs
using Cultural_Heritage_System.Common;
using System.ComponentModel.DataAnnotations;

namespace Cultural_Heritage_System.Dtos.Request.Streaming
{
    public class StreamingRoomCreateRequest
    {
        [Required] public string Title { get; set; } = default!;
    }

    public class StreamingRequestJoinRequest
    {
        // Tuỳ chọn: nếu null/empty => dùng userId (từ claims) làm RtcUid
        public string? RtcUid { get; set; }
    }

    public class StreamingAdmitRejectRequest
    {
        [Required] public int UserId { get; set; }   // user mục tiêu (được host thao tác)
    }

    public class StreamingSetRoleRequest
    {
        [Required] public int UserId { get; set; }
        [Required] public RoomRole Role { get; set; }
    }

    public class StreamingRaiseHandRequest
    {
        [Required] public bool Raised { get; set; }
    }

}

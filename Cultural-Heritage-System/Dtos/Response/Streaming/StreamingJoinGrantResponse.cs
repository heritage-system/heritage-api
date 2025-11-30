namespace Cultural_Heritage_System.Dtos.Response.Streaming
{
    public class StreamingJoinGrantResponse
    {
        public string AppId { get; set; } = default!;   // <-- thêm
        public string Channel { get; set; } = default!;
        public string RtcUid { get; set; } = default!;
        public string Role { get; set; } = default!; // "Host/CoHost/Speaker/Audience"
        public string RtcToken { get; set; } = default!;
        public string RtmToken { get; set; } = default!;
        public string RtmUid { get; set; } = default!;   // 👈 new
                                                         // NEW for screen share
        public string? ScreenRtcUid { get; set; }
        public string? ScreenRtcToken { get; set; }
    }

}

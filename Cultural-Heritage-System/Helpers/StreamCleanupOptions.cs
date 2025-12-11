namespace Cultural_Heritage_System.Helpers
{
    // Helpers/StreamCleanupOptions.cs
    public class StreamCleanupOptions
    {
        public int HeartbeatTtlSeconds { get; set; } = 90;     // quá 90s coi là hết hoạt động
        public int RoomIdleMinutes { get; set; } = 12;         // 12 phút không ai hoạt động → tắt
    }

}

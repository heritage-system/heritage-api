namespace Cultural_Heritage_System.Services
{
    public enum AgoraRtcRole { HOST, AUDIENCE }

    public class AgoraOptions
    {
        public string AppId { get; set; } = default!;
        public string AppCertificate { get; set; } = default!;
        public int DefaultExpirySeconds { get; set; } = 7200;
        // TokenService/Chat có thể bỏ nếu chưa dùng
        public TokenServiceOptions TokenService { get; set; } = new();
        public ChatOptions Chat { get; set; } = new();
        public class TokenServiceOptions { public string BaseUrl { get; set; } = ""; }
        public class ChatOptions { public string OrgName { get; set; } = ""; public string AppName { get; set; } = ""; public string ClientId { get; set; } = ""; public string ClientSecret { get; set; } = ""; }
    }

    public interface IAgoraTokenService
    {
        Task<string> CreateRtcTokenAsync(string channel, string rtcUid, AgoraRtcRole role, int? expireSeconds = null);
        Task<string> CreateRtmTokenAsync(string rtmUserId, int? expireSeconds = null);
        Task<(string rtcToken, string rtmToken)> CreateRteTokensAsync(string channel, string rtcUid, string rtmUserId, AgoraRtcRole role, int? expireSeconds = null);
    }
}

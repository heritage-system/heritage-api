using AgoraIO.Media;     // DynamicKey v1: RtcTokenBuilder
using AgoraIO.Rtm;       // DynamicKey v1: RtmTokenBuilder

using Cultural_Heritage_System.Services;
using Microsoft.Extensions.Options;

public class AgoraTokenService : IAgoraTokenService
{
    private readonly AgoraOptions _opt;
    public AgoraTokenService(IOptions<AgoraOptions> opt) => _opt = opt.Value;

    private uint ExpTs(int? seconds = null)
        => (uint)(DateTimeOffset.UtcNow.ToUnixTimeSeconds() + (seconds ?? _opt.DefaultExpirySeconds));

    // RTC: DynamicKey v1 (6 tham số) - có role
    public Task<string> CreateRtcTokenAsync(string channel, string rtcUid, AgoraRtcRole role, int? expireSeconds = null)
    {
        var rtcRole = role == AgoraRtcRole.HOST
            ? RtcTokenBuilder.Role.RolePublisher
            : RtcTokenBuilder.Role.RoleSubscriber;

        var token = RtcTokenBuilder.buildTokenWithUserAccount(
            _opt.AppId, _opt.AppCertificate,
            channel, rtcUid,
            rtcRole,
            ExpTs(expireSeconds)   // privilegeExpiredTs
        );

        return Task.FromResult(token);
    }

    // RTM: DynamicKey v1 (4 tham số) - KHÔNG có role
    public Task<string> CreateRtmTokenAsync(string rtmUserId, int? expireSeconds = null)
    {
        var token = RtmTokenBuilder.buildToken(
            _opt.AppId,
            _opt.AppCertificate,
            rtmUserId,
            ExpTs(expireSeconds)   // privilegeExpiredTs
        );
        return Task.FromResult(token);
    }

    public async Task<(string rtcToken, string rtmToken)> CreateRteTokensAsync(
        string channel, string rtcUid, string rtmUserId, AgoraRtcRole role, int? expireSeconds = null)
    {
        var rtc = await CreateRtcTokenAsync(channel, rtcUid, role, expireSeconds);
        var rtm = await CreateRtmTokenAsync(rtmUserId, expireSeconds);
        return (rtc, rtm);
    }
}

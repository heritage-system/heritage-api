using Cultural_Heritage_System.Common;
using Cultural_Heritage_System.Dtos.Request.Streaming;
using Cultural_Heritage_System.Dtos.Response;
using Cultural_Heritage_System.Dtos.Response.Streaming;
using Cultural_Heritage_System.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Cultural_Heritage_System.Controllers;

[Route("api/v1/stream")]
[ApiController]
public class StreamingRoomsController : ControllerBase
{
    private readonly IStreamingRoomService _svc;
    private readonly ILogger<StreamingRoomsController> _logger;

    public StreamingRoomsController(
        IStreamingRoomService svc,
        ILogger<StreamingRoomsController> logger)
    {
        _svc = svc;
        _logger = logger;
    }

    // ========== CLIENT JOIN / PARTICIPANTS ==========

    [HttpPost("rooms/{roomName}/join-token")]
    [AllowAnonymous]
    public async Task<ApiResponse<StreamingJoinGrantResponse>> IssueJoinTokens(string roomName)
    {
        var grant = await _svc.IssueJoinTokensAsync(roomName);
        return new ApiResponse<StreamingJoinGrantResponse>(200, "Join tokens issued", grant);
    }

    [HttpGet("rooms/{roomName}/participants")]
    [AllowAnonymous]
    public async Task<ApiResponse<List<StreamingParticipantResponse>>> Participants(
        string roomName, [FromQuery] ParticipantStatus? status)
    {
        var list = await _svc.GetParticipantsAsync(roomName, status);
        return new ApiResponse<List<StreamingParticipantResponse>>(200, "OK", list.ToList());
    }

    [HttpGet("rooms/with-people")]
    [AllowAnonymous]
    public async Task<ApiResponse<List<StreamingRoomWithCountResponse>>> GetRoomsWithPeople(
        [FromQuery] int minCount = 1,
        [FromQuery] ParticipantStatus? status = ParticipantStatus.ADMITTED)
    {
        var list = await _svc.GetRoomsHavingParticipantsAsync(minCount, status);
        return new ApiResponse<List<StreamingRoomWithCountResponse>>(200, "OK", list.ToList());
    }

    [HttpPost("rooms/{roomName}/heartbeat")]
    [AllowAnonymous]
    public async Task<ApiResponse<object>> Heartbeat(string roomName)
    {
        await _svc.HeartbeatAsync(roomName);
        return new ApiResponse<object>(200, "OK", null);
    }

    [HttpPost("rooms/{roomName}/leave")]
    [AllowAnonymous]
    public async Task<ApiResponse<object>> Leave(string roomName)
    {
        await _svc.LeaveAsync(roomName);
        return new ApiResponse<object>(200, "OK", null);
    }

    [HttpPost("rooms/{roomName}/set-role")]
    [AllowAnonymous] // TODO: đổi lại [Authorize] / Roles nếu cần
    public async Task<ApiResponse<object>> SetRole(string roomName, [FromBody] StreamingSetRoleRequest dto)
    {
        await _svc.SetRoleAsync(roomName, dto);
        return new ApiResponse<object>(200, "Role updated", null);
    }

    [HttpPost("rooms/{roomName}/kick")]
    [AllowAnonymous]
    public async Task<ApiResponse<object>> Kick(string roomName, [FromBody] StreamingAdmitRejectRequest dto)
    {
        await _svc.KickAsync(roomName, dto);
        return new ApiResponse<object>(200, "Kicked", null);
    }

    // ========== ADMIN / MANAGEMENT ==========

    // xem chi tiết 1 room (kèm participants)
    [HttpGet("rooms/{roomName}")]
    [AllowAnonymous] // hoặc [Authorize(Roles = "Admin")]
    public async Task<ApiResponse<StreamingRoomDetailResponse>> GetRoomDetail(string roomName)
    {
        var room = await _svc.GetRoomDetailAsync(roomName);
        return new ApiResponse<StreamingRoomDetailResponse>(200, "Get Room Detail", room);
    }

    // update room (title, startAt, type)
    [HttpPut("rooms/{roomName}")]
    [AllowAnonymous] // hoặc [Authorize(Roles = "Admin")]
    public async Task<ApiResponse<StreamingRoomResponse>> UpdateRoom(
        string roomName,
        [FromBody] StreamingRoomUpdateRequest request)
    {
        var room = await _svc.UpdateRoomAsync(roomName, request);
        return new ApiResponse<StreamingRoomResponse>(200, "Room Updated", room);
    }

    // delete room
    [HttpDelete("rooms/{roomName}")]
    [AllowAnonymous] // hoặc [Authorize(Roles = "Admin")]
    public async Task<ApiResponse<object>> DeleteRoom(string roomName)
    {
        await _svc.DeleteRoomAsync(roomName);
        return new ApiResponse<object>(200, "Deleted", null);
    }

    // admin join với role CoHost
    [HttpPost("rooms/{roomName}/admin-join-token")]
    //[Authorize(Roles = "Admin")]
    public async Task<ApiResponse<StreamingJoinGrantResponse>> AdminJoinAsCoHost(string roomName)
    {
        var grant = await _svc.IssueJoinTokensAsync(roomName);
        return new ApiResponse<StreamingJoinGrantResponse>(200, "Join tokens issued", grant);
    }
}

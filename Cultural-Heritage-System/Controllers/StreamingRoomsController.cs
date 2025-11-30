// Controllers/StreamingRoomsController.cs
using Cultural_Heritage_System.Common;
using Cultural_Heritage_System.Dtos.Request.Streaming;
using Cultural_Heritage_System.Dtos.Response;
using Cultural_Heritage_System.Dtos.Response.Streaming;
using Cultural_Heritage_System.Helpers;
using Cultural_Heritage_System.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace Cultural_Heritage_System.Controllers;

[Route("api/v1/stream")]
[ApiController]
public class StreamingRoomsController : ControllerBase
{
    private readonly IStreamingRoomService _svc;
    private readonly ILogger<StreamingRoomsController> _logger;

    public StreamingRoomsController(IStreamingRoomService svc, ILogger<StreamingRoomsController> logger)
    { _svc = svc; _logger = logger; }

    [HttpPost("rooms")]
    //[Authorize]
    [AllowAnonymous]
    public async Task<ApiResponse<StreamingRoomResponse>> CreateRoom([FromBody] StreamingRoomCreateRequest req)
    {
        var room = await _svc.CreateRoomAsync(req);
        return new ApiResponse<StreamingRoomResponse>(
               code: 201,
               message: "Room created",
               result: room
           );

    }

    [HttpPost("rooms/{roomName}/request-join")]
    //[Authorize]
    [AllowAnonymous]
    public async Task<ApiResponse<object>> RequestJoin(string roomName, [FromBody] StreamingRequestJoinRequest dto)
    {
        await _svc.RequestJoinAsync(roomName, dto);
        return new ApiResponse<object>(200, "Requested to join", null);
    }

    [HttpPost("rooms/{roomName}/admit")]
    //[Authorize]
    [AllowAnonymous]
    public async Task<ApiResponse<object>> Admit(string roomName, [FromBody] StreamingAdmitRejectRequest dto)
    {
        await _svc.AdmitAsync(roomName, dto);
        return new ApiResponse<object>(200, "Admitted", null);
    }

    [HttpPost("rooms/{roomName}/reject")]
    //[Authorize]
    [AllowAnonymous]
    public async Task<ApiResponse<object>> Reject(string roomName, [FromBody] StreamingAdmitRejectRequest dto)
    {
        await _svc.RejectAsync(roomName, dto);
        return new ApiResponse<object>(200, "Rejected", null);
    }

    [HttpPost("rooms/{roomName}/set-role")]
    //[Authorize]
    [AllowAnonymous]
    public async Task<ApiResponse<object>> SetRole(string roomName, [FromBody] StreamingSetRoleRequest dto)
    {
        await _svc.SetRoleAsync(roomName, dto);
        return new ApiResponse<object>(200, "Role updated", null);
    }

    [HttpPost("rooms/{roomName}/raise-hand")]
    [AllowAnonymous]
    //[Authorize]
    public async Task<ApiResponse<object>> RaiseHand(string roomName, [FromBody] StreamingRaiseHandRequest dto)
    {
        await _svc.RaiseHandAsync(roomName, dto);
        return new ApiResponse<object>(200, "Raise hand updated", null);
    }
    // Controllers/StreamingRoomsController.cs
    [HttpGet("rooms/{roomName}/participants")]
    [AllowAnonymous]
    public async Task<ApiResponse<List<StreamingParticipantResponse>>> Participants(
        string roomName, [FromQuery] ParticipantStatus? status)
    {
        var list = await _svc.GetParticipantsAsync(roomName, status);
        return new ApiResponse<List<StreamingParticipantResponse>>(200, "OK", list.ToList());
    }

    [HttpPost("rooms/{roomName}/join-token")]
    [AllowAnonymous]
    //[Authorize]
    public async Task<ApiResponse<StreamingJoinGrantResponse>> IssueJoinTokens(string roomName)
    {
        var grant = await _svc.IssueJoinTokensAsync(roomName);
        return new ApiResponse<StreamingJoinGrantResponse>(200, "Join tokens issued", grant);
    }
    [HttpGet("rooms/{roomName}/waiting")]
    [AllowAnonymous]
    public async Task<ApiResponse<List<StreamingParticipantResponse>>> Waiting(string roomName)
    {
        var list = await _svc.GetWaitingListAsync(roomName);
        return new ApiResponse<List<StreamingParticipantResponse>>(200, "OK", list.ToList());
    }
    [HttpGet("config")]
    [AllowAnonymous]
    public ActionResult<StreamAdmissionOptions> GetConfig(
    [FromServices] IOptions<StreamAdmissionOptions> opt)
    {
        return Ok(new StreamAdmissionOptions { OpenAdmission = opt.Value.OpenAdmission });
    }
    [HttpGet("rooms/with-people")]
    [AllowAnonymous]
    public async Task<ApiResponse<List<StreamingRoomWithCountResponse>>> GetRoomsWithPeople(
     [FromQuery] int minCount = 1,
     [FromQuery] ParticipantStatus? status = ParticipantStatus.Admitted)
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
    [HttpPost("rooms/{roomName}/kick")]
    [AllowAnonymous] // (đổi thành [Authorize] khi bạn bật auth)
    public async Task<ApiResponse<object>> Kick(string roomName, [FromBody] StreamingAdmitRejectRequest dto)
    {
        await _svc.KickAsync(roomName, dto);
        return new ApiResponse<object>(200, "Kicked", null);
    }
}

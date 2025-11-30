// Services/IStreamingRoomService.cs
using Cultural_Heritage_System.Common;
using Cultural_Heritage_System.Dtos.Request.Streaming;
using Cultural_Heritage_System.Dtos.Response.Streaming;

namespace Cultural_Heritage_System.Services
{
    public interface IStreamingRoomService
    {
        Task<StreamingRoomResponse> CreateRoomAsync(StreamingRoomCreateRequest request);

        Task RequestJoinAsync(string roomName, StreamingRequestJoinRequest request);
        Task AdmitAsync(string roomName, StreamingAdmitRejectRequest request);   // target userId vẫn nằm trong dto
        Task RejectAsync(string roomName, StreamingAdmitRejectRequest request);

        Task SetRoleAsync(string roomName, StreamingSetRoleRequest request);
        Task RaiseHandAsync(string roomName, StreamingRaiseHandRequest request);

        Task<StreamingJoinGrantResponse> IssueJoinTokensAsync(string roomName);
        Task<IReadOnlyList<StreamingParticipantResponse>> GetParticipantsAsync(string roomName, ParticipantStatus? status);

        Task<IReadOnlyList<StreamingParticipantResponse>> GetWaitingListAsync(string roomName);
        Task<IReadOnlyList<StreamingRoomWithCountResponse>> GetRoomsHavingParticipantsAsync(
          int minCount = 1, ParticipantStatus? status = ParticipantStatus.Admitted);

        Task KickAsync(string roomName, StreamingAdmitRejectRequest request);
        Task HeartbeatAsync(string roomName);
        Task LeaveAsync(string roomName);

    }
}

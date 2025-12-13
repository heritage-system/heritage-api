using Cultural_Heritage_System.Common;
using Cultural_Heritage_System.Dtos.Request.Streaming;
using Cultural_Heritage_System.Dtos.Response.Streaming;

namespace Cultural_Heritage_System.Services
{
    public interface IStreamingRoomService
    {


        // 🔥 Join thẳng, không request/admit/reject
        Task<StreamingJoinGrantResponse> IssueJoinTokensAsync(string roomName);

        Task<IReadOnlyList<StreamingParticipantResponse>> GetParticipantsAsync(
            string roomName, ParticipantStatus? status);

        Task<IReadOnlyList<StreamingRoomWithCountResponse>> GetRoomsHavingParticipantsAsync(
            int minCount = 1, ParticipantStatus? status = ParticipantStatus.ADMITTED);

        // Sau khi đã vào phòng: host/cohost có thể set role hoặc kick
        Task SetRoleAsync(string roomName, StreamingSetRoleRequest request);
        Task KickAsync(string roomName, StreamingAdmitRejectRequest request);

        // Giữ heartbeat / leave để track số người
        Task HeartbeatAsync(string roomName);
        Task LeaveAsync(string roomName);

        // 🔥 Optional: người dùng "đăng ký" event

        Task<StreamingRoomDetailResponse> GetRoomDetailAsync(string roomName);
        Task<StreamingRoomResponse> UpdateRoomAsync(string roomName, StreamingRoomUpdateRequest request);
        Task DeleteRoomAsync(string roomName);



    }
}

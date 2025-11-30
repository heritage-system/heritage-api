using Cultural_Heritage_System.Common;
using Cultural_Heritage_System.Models;

namespace Cultural_Heritage_System.Repositories
{
    public interface IStreamingRoomRepository : IBaseRepository<StreamingRoom>
    {
        public class RoomWithCount // POCO trung gian cho repo
        {
            public StreamingRoom Room { get; set; } = default!;
            public int Count { get; set; }
        }
        Task<StreamingRoom?> GetByRoomName(string roomName);
        Task<List<StreamingRoom>> GetRooms(int page, int size);
        // NEW: trả danh sách phòng có số người theo status >= minCount
        Task<List<RoomWithCount>> GetRoomsHavingParticipantsAsync(
     int minCount = 1, ParticipantStatus? status = ParticipantStatus.Admitted);
        Task<List<StreamingRoom>> GetActiveRoomsAsync();
    }
    public interface IStreamingParticipantRepository : IBaseRepository<StreamingParticipant>
    {
        Task<StreamingParticipant?> GetByRoomAndUser(int roomId, int userId);
        Task<List<StreamingParticipant>> GetWaitingList(int roomId);
        Task<List<StreamingParticipant>> GetByRoom(int roomId, ParticipantStatus? status);
        Task TouchLastSeenAsync(int roomId, int userId);
        Task MarkLeftAsync(int roomId, int userId, bool setStatusLeft = true);

    }
    public interface IRaiseHandRepository : IBaseRepository<RaiseHandRequest>
    {
        Task<List<RaiseHandRequest>> GetPending(int roomId);
    }
    public interface IRoomChatRepository : IBaseRepository<RoomChatMessage>
    {
        Task<List<RoomChatMessage>> GetRecent(int roomId, int take = 100);
    }
}

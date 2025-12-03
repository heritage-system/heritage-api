using Cultural_Heritage_System.Common;
using Cultural_Heritage_System.Models;

namespace Cultural_Heritage_System.Repositories
{
    public interface IStreamingRoomRepository : IBaseRepository<StreamingRoom>
    {
        public class RoomWithCount
        {
            public StreamingRoom Room { get; set; } = default!;
            public int Count { get; set; }
        }

        Task<StreamingRoom?> GetByRoomName(string roomName);
        Task<List<StreamingRoom>> GetRooms(int page, int size);
        Task<List<RoomWithCount>> GetRoomsHavingParticipantsAsync(
            int minCount = 1, ParticipantStatus? status = ParticipantStatus.ADMITTED);

        Task<List<StreamingRoom>> GetActiveRoomsAsync();
        Task<List<StreamingRoom>> GetUpcomingRoomsAsync(DateTime fromUtc, int max = 20);
    }

    public interface IStreamingParticipantRepository : IBaseRepository<StreamingParticipant>
    {
        Task<StreamingParticipant?> GetByRoomAndUser(int roomId, int userId);
        Task<List<StreamingParticipant>> GetByRoom(int roomId, ParticipantStatus? status);
        Task TouchLastSeenAsync(int roomId, int userId);
        Task MarkLeftAsync(int roomId, int userId, bool setStatusLeft = true);

    }
}

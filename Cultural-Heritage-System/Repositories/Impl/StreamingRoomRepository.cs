using Cultural_Heritage_System.Common;
using Cultural_Heritage_System.DataAccessObjects;
using Cultural_Heritage_System.Models;
using static Cultural_Heritage_System.Repositories.IStreamingRoomRepository;

namespace Cultural_Heritage_System.Repositories.Impl
{
    public class StreamingRoomRepository : BaseRepository<StreamingRoom>, IStreamingRoomRepository
    {
        private readonly StreamingRoomDAO _dao;
        public StreamingRoomRepository(StreamingRoomDAO dao) : base(dao) => _dao = dao;
        public Task<StreamingRoom?> GetByRoomName(string roomName) => _dao.GetByRoomName(roomName);
        public Task<List<StreamingRoom>> GetRooms(int page, int size) => _dao.GetRooms(page, size);
        public Task<List<RoomWithCount>> GetRoomsHavingParticipantsAsync(
         int minCount = 1, ParticipantStatus? status = ParticipantStatus.ADMITTED) =>
         _dao.GetRoomsHavingParticipantsAsync(minCount, status);
        // Repositories/Impl/StreamingRoomRepository.cs
        public Task<List<StreamingRoom>> GetActiveRoomsAsync() => _dao.GetActiveRoomsAsync();
        public Task<List<StreamingRoom>> GetUpcomingRoomsAsync(DateTime fromUtc, int max = 20)
       => _dao.GetUpcomingRoomsAsync(fromUtc, max);

    }

    public class StreamingParticipantRepository : BaseRepository<StreamingParticipant>, IStreamingParticipantRepository
    {
        private readonly StreamingParticipantDAO _dao;
        public StreamingParticipantRepository(StreamingParticipantDAO dao) : base(dao) => _dao = dao;
        public Task<StreamingParticipant?> GetByRoomAndUser(int roomId, int userId) => _dao.GetByRoomAndUser(roomId, userId);


        public Task TouchLastSeenAsync(int roomId, int userId) => _dao.TouchLastSeenAsync(roomId, userId);
        public Task MarkLeftAsync(int roomId, int userId, bool setStatusLeft = true) => _dao.MarkLeftAsync(roomId, userId);

        public Task<List<StreamingParticipant>> GetByRoom(int roomId, ParticipantStatus? status) => _dao.GetByRoom(roomId, status);

    }


}

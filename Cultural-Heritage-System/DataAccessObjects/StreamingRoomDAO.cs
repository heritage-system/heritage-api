using Cultural_Heritage_System.Common;
using Cultural_Heritage_System.Models;
using Microsoft.EntityFrameworkCore;
using static Cultural_Heritage_System.Repositories.IStreamingRoomRepository;

namespace Cultural_Heritage_System.DataAccessObjects
{
    public class StreamingRoomDAO : BaseDAO<StreamingRoom>
    {
        private readonly ILogger<StreamingRoomDAO> _logger;

        public StreamingRoomDAO(AppDbContext context, ILogger<StreamingRoomDAO> logger)
            : base(context)
        {
            _logger = logger;
        }


        public async Task<StreamingRoom?> GetByRoomName(string roomName) =>
            await _dbSet.Include(r => r.Participants).ThenInclude(p => p.User)
                  .FirstOrDefaultAsync(r => r.RoomName == roomName);
        public async Task<List<StreamingRoom>> GetActiveRoomsAsync() =>
    await _dbSet.Where(r => r.IsActive).ToListAsync();
        public async Task<List<StreamingRoom>> GetUpcomingRoomsAsync(DateTime fromUtc, int max = 20)
        {
            return await _dbSet
                .Where(r => r.StartAt >= fromUtc && !r.IsActive)
                .OrderBy(r => r.StartAt)
                .Take(max)
                .ToListAsync();
        }
        public async Task<List<StreamingRoom>> GetRooms(int page, int size) =>
            await _dbSet.OrderByDescending(x => x.CreatedAt)
                  .Skip((page - 1) * size)
                  .Take(size).ToListAsync();
        public async Task<List<RoomWithCount>> GetRoomsHavingParticipantsAsync(
          int minCount = 1, ParticipantStatus? status = ParticipantStatus.ADMITTED)
        {
            // Đếm participants theo status (nếu null thì đếm tất cả)
            var countsQry =
                _context.Set<StreamingParticipant>()
                    .Where(p => status == null || p.Status == status)
                    .GroupBy(p => p.RoomId)
                    .Select(g => new { RoomId = g.Key, Count = g.Count() })
                    .Where(x => x.Count >= minCount);

            var query =
                from r in _dbSet
                join c in countsQry on r.Id equals c.RoomId
                orderby r.CreatedAt descending
                select new RoomWithCount { Room = r, Count = c.Count };

            return await query.ToListAsync();
        }


    }

    public class StreamingParticipantDAO : BaseDAO<StreamingParticipant>
    {
        private readonly ILogger<StreamingParticipantDAO> _logger;

        public StreamingParticipantDAO(AppDbContext context, ILogger<StreamingParticipantDAO> logger)
            : base(context)
        {
            _logger = logger;
        }

        public async Task<List<StreamingParticipant>> GetByRoom(int roomId, ParticipantStatus? status) =>
            await _dbSet.Include(p => p.User).Include(p => p.Room)
                .Where(p => p.RoomId == roomId && (status == null || p.Status == status))
                .OrderBy(p => p.CreatedAt)
                .ToListAsync();

        public async Task<StreamingParticipant?> GetByRoomAndUser(int roomId, int userId) =>
            await _dbSet.Include(p => p.User).Include(p => p.Room)
                .FirstOrDefaultAsync(p => p.RoomId == roomId && p.UserId == userId);

        public async Task TouchLastSeenAsync(int roomId, int userId)
        {
            var sp = await _dbSet.FirstOrDefaultAsync(p => p.RoomId == roomId && p.UserId == userId);
            if (sp == null) return;
            sp.LastSeenAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();
        }

        public async Task MarkLeftAsync(int roomId, int userId, bool setStatusLeft = true)
        {
            var sp = await _dbSet.FirstOrDefaultAsync(p => p.RoomId == roomId && p.UserId == userId);
            if (sp == null) return;

            sp.LeftAt = DateTime.UtcNow;
            sp.LastSeenAt = sp.LeftAt;

            if (setStatusLeft && Enum.IsDefined(typeof(ParticipantStatus), "LEFT"))
            {
                sp.Status = ParticipantStatus.LEFT;
            }

            await _context.SaveChangesAsync();
        }
    }

}

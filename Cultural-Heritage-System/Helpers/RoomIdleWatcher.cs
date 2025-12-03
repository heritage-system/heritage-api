using Cultural_Heritage_System.Common;
using Cultural_Heritage_System.Repositories;
using Microsoft.Extensions.Options;

namespace Cultural_Heritage_System.Helpers
{
    public class RoomIdleWatcher : BackgroundService
    {
        private readonly ILogger<RoomIdleWatcher> _logger;
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly StreamCleanupOptions _opt;

        public RoomIdleWatcher(
            ILogger<RoomIdleWatcher> logger,
            IServiceScopeFactory scopeFactory,
            IOptions<StreamCleanupOptions> opt)
        {
            _logger = logger;
            _scopeFactory = scopeFactory;
            _opt = opt.Value;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            var timer = new PeriodicTimer(TimeSpan.FromSeconds(60));
            while (!stoppingToken.IsCancellationRequested
                   && await timer.WaitForNextTickAsync(stoppingToken))
            {
                try { await SweepAsync(stoppingToken); }
                catch (Exception ex) { _logger.LogError(ex, "RoomIdleWatcher sweep failed"); }
            }
        }

        private async Task SweepAsync(CancellationToken ct)
        {
            using var scope = _scopeFactory.CreateScope();
            var roomRepo = scope.ServiceProvider.GetRequiredService<IStreamingRoomRepository>();
            var partRepo = scope.ServiceProvider.GetRequiredService<IStreamingParticipantRepository>();

            var now = DateTime.UtcNow;
            var activeRooms = await roomRepo.GetActiveRoomsAsync(); // thêm ở repo (bên dưới)

            var recentCut = now.AddSeconds(-_opt.HeartbeatTtlSeconds);
            foreach (var room in activeRooms)
            {
                var parts = await partRepo.GetByRoom(room.Id, ParticipantStatus.ADMITTED);
                var anyActive = parts.Any(p => p.LastSeenAt.HasValue && p.LastSeenAt.Value > recentCut);
                if (anyActive) continue;

                if (!anyActive)
                {
                    var lastSeen = parts.Max(p => p.LastSeenAt);
                    DateTime lastSeenMax = lastSeen ?? ((room.UpdatedAt != default) ? room.UpdatedAt : room.CreatedAt);

                    if (lastSeenMax <= now.AddMinutes(-_opt.RoomIdleMinutes))
                    {
                        room.IsActive = false;
                        room.Type = StreamingRoomType.CLOSED;      // 🔥 NEW
                        room.ClosedAt = now;
                        await roomRepo.UpdateAsync(room);
                        _logger.LogInformation("Deactivated room {Room}", room.RoomName);
                    }
                }
            }
        }
    }

}

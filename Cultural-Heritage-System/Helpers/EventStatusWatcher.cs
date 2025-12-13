// Helpers/EventStatusWatcher.cs
using Cultural_Heritage_System.Common;
using Cultural_Heritage_System.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Cultural_Heritage_System.Helpers
{
    public class EventStatusWatcher : BackgroundService
    {
        private readonly ILogger<EventStatusWatcher> _logger;
        private readonly IServiceScopeFactory _scopeFactory;

        public EventStatusWatcher(
            ILogger<EventStatusWatcher> logger,
            IServiceScopeFactory scopeFactory)
        {
            _logger = logger;
            _scopeFactory = scopeFactory;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            // quét mỗi 60s (tuỳ chỉnh)
            var timer = new PeriodicTimer(TimeSpan.FromSeconds(60));

            while (!stoppingToken.IsCancellationRequested &&
                   await timer.WaitForNextTickAsync(stoppingToken))
            {
                try
                {
                    await SweepAsync(stoppingToken);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "EventStatusWatcher sweep failed");
                }
            }
        }

        private async Task SweepAsync(CancellationToken ct)
        {
            using var scope = _scopeFactory.CreateScope();
            var eventRepo = scope.ServiceProvider.GetRequiredService<IEventRepository>();

            var nowUtc = DateTime.UtcNow;


            var q = eventRepo.GetEventsWithIncludes(); // IQueryable<Event>

            // tìm những event cần đổi status
            var candidates = await q
                .Where(e =>
                    (e.Status == EventStatus.UPCOMING && e.StartAt <= nowUtc) ||
                    (e.Status == EventStatus.LIVE &&
                     e.CloseAt.HasValue &&
                     e.CloseAt <= nowUtc)
                )
                .ToListAsync(ct);

            if (!candidates.Any()) return;

            foreach (var ev in candidates)
            {
                if (ev.Status == EventStatus.UPCOMING && ev.StartAt <= nowUtc)
                {
                    ev.Status = EventStatus.LIVE;
                    _logger.LogInformation("Event {Id} switched UPCOMING -> LIVE", ev.Id);
                }

                if (ev.Status == EventStatus.LIVE &&
                    ev.CloseAt.HasValue &&
                    ev.CloseAt <= nowUtc)
                {
                    ev.Status = EventStatus.CLOSED;
                    _logger.LogInformation("Event {Id} switched LIVE -> CLOSED", ev.Id);
                }

                await eventRepo.UpdateAsync(ev);
            }
        }
    }
}

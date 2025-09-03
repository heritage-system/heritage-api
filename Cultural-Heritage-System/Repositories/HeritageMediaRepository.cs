using Cultural_Heritage_System.Models;
using medical_appointment_booking.Repositories;

namespace Cultural_Heritage_System.Repositories
{
    public class HeritageMediaRepository : BaseRepository<HeritageMedia>
    {
        private readonly ILogger<HeritageMediaRepository> _logger;

        public HeritageMediaRepository(AppDbContext context, ILogger<HeritageMediaRepository> logger)
            : base(context)
        {
            _logger = logger;
        }

        public async Task AddRangeAsync(IEnumerable<HeritageMedia> entities)
        {
            await _dbSet.AddRangeAsync(entities);
            await SaveChangesAsync();
        }

    }
}

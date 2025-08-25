using Cultural_Heritage_System.Models;
using medical_appointment_booking.Repositories;

namespace Cultural_Heritage_System.Repositories
{
    public class HeritageTagRepository : BaseRepository<HeritageTag>
    {
        private readonly ILogger<HeritageTagRepository> _logger;

        public HeritageTagRepository(AppDbContext context, ILogger<HeritageTagRepository> logger)
            : base(context)
        {
            _logger = logger;
        }

        public async Task AddRangeAsync(IEnumerable<HeritageTag> entities)
        {
            await _dbSet.AddRangeAsync(entities);
            await SaveChangesAsync();
        }
    }
}


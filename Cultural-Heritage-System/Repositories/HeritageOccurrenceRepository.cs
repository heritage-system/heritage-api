using Cultural_Heritage_System.Models;
using Cultural_Heritage_System.Repositories;

namespace Cultural_Heritage_System.Repositories
{
    public class HeritageOccurrenceRepository : BaseRepository<HeritageOccurrence>
    {
        private readonly ILogger<HeritageOccurrenceRepository> _logger;

        public HeritageOccurrenceRepository(AppDbContext context, ILogger<HeritageOccurrenceRepository> logger)
            : base(context)
        {
            _logger = logger;
        }

        public async Task AddRangeAsync(IEnumerable<HeritageOccurrence> entities)
        {
            await _dbSet.AddRangeAsync(entities);
            await SaveChangesAsync();
        }
    }
}

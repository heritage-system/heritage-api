using Cultural_Heritage_System.Models;
using Cultural_Heritage_System.Repositories;

namespace Cultural_Heritage_System.Repositories
{
    public class HeritageLocationRepository : BaseRepository<HeritageLocation>
    {
        private readonly ILogger<HeritageLocationRepository> _logger;

        public HeritageLocationRepository(AppDbContext context, ILogger<HeritageLocationRepository> logger)
            : base(context)
        {
            _logger = logger;
        }

        public async Task AddRangeAsync(IEnumerable<HeritageLocation> heritageLocations)
        {
            await _dbSet.AddRangeAsync(heritageLocations);
            await SaveChangesAsync();
        }
    }
}

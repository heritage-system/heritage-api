using Cultural_Heritage_System.Models;
using Cultural_Heritage_System.Repositories;

namespace Cultural_Heritage_System.Repositories
{
    public class LocationRepository : BaseRepository<Location>
    {
        private readonly ILogger<LocationRepository> _logger;

        public LocationRepository(AppDbContext context, ILogger<LocationRepository> logger)
            : base(context)
        {
            _logger = logger;
        }

        public async Task AddRangeAsync(IEnumerable<Location> heritageLocations)
        {
            await _dbSet.AddRangeAsync(heritageLocations);
            await SaveChangesAsync();
        }
    }
}

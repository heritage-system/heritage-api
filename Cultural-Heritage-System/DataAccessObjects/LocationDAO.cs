using Cultural_Heritage_System.Models;
using Cultural_Heritage_System.Repositories;

namespace Cultural_Heritage_System.DataAccessObjects
{
    public class LocationDAO : BaseDAO<Location>
    {
        private readonly ILogger<LocationDAO> _logger;

        public LocationDAO(AppDbContext context, ILogger<LocationDAO> logger)
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

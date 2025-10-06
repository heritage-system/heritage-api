using Cultural_Heritage_System.Models;
using Cultural_Heritage_System.Repositories;

namespace Cultural_Heritage_System.DataAccessObjects
{
    public class HeritageLocationDAO : BaseDAO<HeritageLocation>
    {
        private readonly ILogger<HeritageLocationDAO> _logger;

        public HeritageLocationDAO(AppDbContext context, ILogger<HeritageLocationDAO> logger)
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

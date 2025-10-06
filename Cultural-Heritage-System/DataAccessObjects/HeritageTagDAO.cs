using Cultural_Heritage_System.Models;
using Cultural_Heritage_System.Repositories;

namespace Cultural_Heritage_System.DataAccessObjects
{
    public class HeritageTagDAO : BaseDAO<HeritageTag>
    {
        private readonly ILogger<HeritageTagDAO> _logger;

        public HeritageTagDAO(AppDbContext context, ILogger<HeritageTagDAO> logger)
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


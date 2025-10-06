using Cultural_Heritage_System.Models;
using Cultural_Heritage_System.Repositories;

namespace Cultural_Heritage_System.DataAccessObjects
{
    public class HeritageMediaDAO : BaseDAO<HeritageMedia>
    {
        private readonly ILogger<HeritageMediaDAO> _logger;

        public HeritageMediaDAO(AppDbContext context, ILogger<HeritageMediaDAO> logger)
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

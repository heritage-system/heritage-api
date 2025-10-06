using Cultural_Heritage_System.Models;
using Cultural_Heritage_System.Repositories;

namespace Cultural_Heritage_System.DataAccessObjects
{
    public class HeritageOccurrenceDAO : BaseDAO<HeritageOccurrence>
    {
        private readonly ILogger<HeritageOccurrenceDAO> _logger;

        public HeritageOccurrenceDAO(AppDbContext context, ILogger<HeritageOccurrenceDAO> logger)
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

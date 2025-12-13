using Cultural_Heritage_System.Models;
using Cultural_Heritage_System.Repositories;
using Cultural_Heritage_System.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Cultural_Heritage_System.DataAccessObjects
{
    public class HeritageDAO : BaseDAO<Heritage>
    {
        private readonly ILogger<HeritageDAO> _logger;

        public HeritageDAO(AppDbContext context, ILogger<HeritageDAO> logger)
            : base(context)
        {
            _logger = logger;
        }

        public override async Task<IEnumerable<Heritage>> GetAllAsync()
        {
            return await _dbSet
                .Include(h => h.Category)
                .Include(h => h.Media)
                .Include(h => h.HeritageTags)
                    .ThenInclude(ht => ht.Tag)
                .Include(h => h.HeritageLocations)
                    .ThenInclude(hl => hl.Location)
                .Include(h => h.HeritageOccurrences)
                .ToListAsync();
        }

        public override async Task<Heritage> GetByIdAsync(object id)
        {
            long heritageId = id is long l ? l : Convert.ToInt64(id);
            return await _dbSet
                .Where(h => h.Id == heritageId)
                .Include(h => h.Category)
                .Include(h => h.Media)
                .Include(h => h.HeritageTags)
                    .ThenInclude(ht => ht.Tag)
                .Include(h => h.HeritageLocations)
                    .ThenInclude(hl => hl.Location)
                .Include(h => h.HeritageOccurrences)
                .FirstOrDefaultAsync();
        }

        public IQueryable<Heritage> GetAllQuery()
        {
            return _dbSet
                .Include(h => h.Category)
                .Include(h => h.Media)
                .Include(h => h.HeritageTags)
                    .ThenInclude(ht => ht.Tag)
                .Include(h => h.HeritageLocations)
                    .ThenInclude(hl => hl.Location)
                .Include(h => h.HeritageOccurrences);
        }

        public async Task<bool> HeritageExistsAsync(long heritageId)
        {
            return await _dbSet.AnyAsync(h => h.Id == heritageId);
        }

        public async Task<Heritage> GetHeritageByIdAsync(long heritageId)
        {
            return await _dbSet
                .Include(h => h.Category)
                .FirstOrDefaultAsync(h => h.Id == heritageId);
        }

        public IQueryable<Heritage> GetHeritagesQueryable()
        {
            return _context.Heritages
                .Include(h => h.Category)
                .Include(h => h.HeritageTags).ThenInclude(ht => ht.Tag)
                .Include(h => h.HeritageOccurrences)
                .Include(h => h.Media)
                .Include(h => h.HeritageLocations).ThenInclude(hl => hl.Location)
                .AsQueryable();
        }

        public async Task<Heritage?> GetHeritageById(long id)
        {
            return await _dbSet
                .Include(h => h.Category)
                .Include(h => h.HeritageTags).ThenInclude(ht => ht.Tag)
                .Include(h => h.HeritageOccurrences)
                .Include(h => h.Media)
                .Include(h => h.HeritageLocations).ThenInclude(hl => hl.Location)
                .FirstOrDefaultAsync(u => u.Id == id);
        }

    }
}

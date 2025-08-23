using Cultural_Heritage_System.Models;
using medical_appointment_booking.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Cultural_Heritage_System.Repositories
{
    public class HeritageRepository : BaseRepository<Heritage>
    {
        private readonly ILogger<HeritageRepository> _logger;

        public HeritageRepository(AppDbContext context, ILogger<HeritageRepository> logger)
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
                .Include(h => h.Coordinates)
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
                .Include(h => h.Coordinates)
                .FirstOrDefaultAsync();
        }
    }
}

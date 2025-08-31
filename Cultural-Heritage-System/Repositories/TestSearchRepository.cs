using CloudinaryDotNet.Core;
using Cultural_Heritage_System.Common;
using Cultural_Heritage_System.Models;
using medical_appointment_booking.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Numerics;
using System.Threading.Tasks;

namespace Cultural_Heritage_System.Repositories
{
    public class TestSearchRepository : BaseRepository<Heritage>
    {
        private readonly ILogger<TestSearchRepository> _logger;

        public TestSearchRepository(AppDbContext context, ILogger<TestSearchRepository> logger)
            : base(context)
        {
            _logger = logger;
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

using Cultural_Heritage_System.Common;
using Cultural_Heritage_System.Dtos.Models;
using Cultural_Heritage_System.Models;
using Cultural_Heritage_System.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Cultural_Heritage_System.DataAccessObjects
{
    public class PanoramaTourDAO : BaseDAO<PanoramaTour>
    {
        private readonly ILogger<PanoramaTourDAO> _logger;

        public PanoramaTourDAO(AppDbContext context, ILogger<PanoramaTourDAO> logger)
            : base(context)
        {
            _logger = logger;
        }

        public IQueryable<PanoramaTour> GetPanoramaToursQueryable()
        {
            return _dbSet
                .Include(p => p.Heritage)
                .ThenInclude(h => h.HeritageLocations)
                .ThenInclude(l => l.Location)
                .Include(p => p.Scenes)                
                .AsQueryable();
        }

        public async Task<PanoramaTour?> GetPanoramaTourById(long id)
        {
            return await _dbSet
                .Include(p => p.Heritage)
               .Include(p => p.Scenes)              
               .FirstOrDefaultAsync(u => u.Id == id);
        }        

    }
}
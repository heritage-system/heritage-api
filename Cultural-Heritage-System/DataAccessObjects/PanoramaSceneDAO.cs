using Cultural_Heritage_System.Common;
using Cultural_Heritage_System.Dtos.Models;
using Cultural_Heritage_System.Models;
using Cultural_Heritage_System.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Cultural_Heritage_System.DataAccessObjects
{
    public class PanoramaSceneDAO : BaseDAO<PanoramaScene>
    {
        private readonly ILogger<PanoramaSceneDAO> _logger;

        public PanoramaSceneDAO(AppDbContext context, ILogger<PanoramaSceneDAO> logger)
            : base(context)
        {
            _logger = logger;
        }

        public IQueryable<PanoramaScene> GetPanoramaScenesQueryable()
        {
            return _dbSet          
                .Include(h => h.InteractionPoints)               
                .AsQueryable();
        }

        public async Task<PanoramaScene?> GetPanoramaSceneById(long id)
        {
            return await _dbSet          
               .Include(h => h.InteractionPoints)
               .FirstOrDefaultAsync(u => u.Id == id);
        }        

    }
}
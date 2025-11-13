using Cultural_Heritage_System.Common;
using Cultural_Heritage_System.Dtos.Models;
using Cultural_Heritage_System.Models;
using Cultural_Heritage_System.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Cultural_Heritage_System.DataAccessObjects
{
    public class PanoramaInteractionPointDAO : BaseDAO<PanoramaInteractionPoint>
    {
        private readonly ILogger<PanoramaInteractionPointDAO> _logger;

        public PanoramaInteractionPointDAO(AppDbContext context, ILogger<PanoramaInteractionPointDAO> logger)
            : base(context)
        {
            _logger = logger;
        }
   
        public async Task<PanoramaInteractionPoint?> GetPanoramaInteractionPointById(long id)
        {
            return await _dbSet             
               .FirstOrDefaultAsync(u => u.Id == id);
        }        

    }
}
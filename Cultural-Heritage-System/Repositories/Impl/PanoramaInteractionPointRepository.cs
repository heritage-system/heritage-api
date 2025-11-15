using Cultural_Heritage_System.Common;
using Cultural_Heritage_System.DataAccessObjects;
using Cultural_Heritage_System.Dtos.Models;
using Cultural_Heritage_System.Models;
using Cultural_Heritage_System.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Cultural_Heritage_System.Repositories.Impl
{
    public class PanoramaInteractionPointRepository : BaseRepository<PanoramaInteractionPoint>, IPanoramaInteractionPointRepository
    {
        private readonly PanoramaInteractionPointDAO _entityDAO;

        public PanoramaInteractionPointRepository(PanoramaInteractionPointDAO dao) : base(dao)
        {
            _entityDAO = dao;
        }
     
        public async Task<PanoramaInteractionPoint?> GetPanoramaInteractionPointById(long id)
        {
            return await _entityDAO.GetPanoramaInteractionPointById(id);
        }    
             
    }
}
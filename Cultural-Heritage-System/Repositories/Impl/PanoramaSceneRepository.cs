using Cultural_Heritage_System.Common;
using Cultural_Heritage_System.DataAccessObjects;
using Cultural_Heritage_System.Dtos.Models;
using Cultural_Heritage_System.Models;
using Cultural_Heritage_System.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Cultural_Heritage_System.Repositories.Impl
{
    public class PanoramaSceneRepository : BaseRepository<PanoramaScene>, IPanoramaSceneRepository
    {
        private readonly PanoramaSceneDAO _entityDAO;

        public PanoramaSceneRepository(PanoramaSceneDAO dao) : base(dao)
        {
            _entityDAO = dao;
        }

        public IQueryable<PanoramaScene> GetPanoramaScenesQueryable()
        {
            return _entityDAO.GetPanoramaScenesQueryable();
        }
      
        public async Task<PanoramaScene?> GetPanoramaSceneById(long id)
        {
            return await _entityDAO.GetPanoramaSceneById(id);
        }    
             
    }
}
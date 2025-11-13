using Cultural_Heritage_System.Common;
using Cultural_Heritage_System.DataAccessObjects;
using Cultural_Heritage_System.Dtos.Models;
using Cultural_Heritage_System.Models;
using Cultural_Heritage_System.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Cultural_Heritage_System.Repositories.Impl
{
    public class PanoramaTourRepository : BaseRepository<PanoramaTour>, IPanoramaTourRepository
    {
        private readonly PanoramaTourDAO _entityDAO;

        public PanoramaTourRepository(PanoramaTourDAO dao) : base(dao)
        {
            _entityDAO = dao;
        }

        public IQueryable<PanoramaTour> GetPanoramaToursQueryable()
        {
            return _entityDAO.GetPanoramaToursQueryable();
        }
      
        public async Task<PanoramaTour?> GetPanoramaTourById(long id)
        {
            return await _entityDAO.GetPanoramaTourById(id);
        }    
             
    }
}
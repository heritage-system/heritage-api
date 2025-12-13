using Cultural_Heritage_System.DataAccessObjects;
using Cultural_Heritage_System.Models;
using Cultural_Heritage_System.Repositories;
using Cultural_Heritage_System.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Cultural_Heritage_System.Repositories.Impl
{
    public class HeritageRepository : BaseRepository<Heritage>, IHeritageRepository
    {
        private readonly HeritageDAO _entityDAO;

        public HeritageRepository(HeritageDAO dao) : base(dao)
        {
            _entityDAO = dao;
        }

        public async Task<IEnumerable<Heritage>> GetAllHeritageAsync()
        {
            return await _entityDAO.GetAllAsync();
        }
       
        public IQueryable<Heritage> GetAllQuery()
        {
            return _entityDAO.GetAllQuery();
        }

        public async Task<bool> HeritageExistsAsync(long heritageId)
        {
            return await _entityDAO.HeritageExistsAsync(heritageId);
        }

        public async Task<Heritage> GetHeritageByIdAsync(long heritageId)
        {
            return await _entityDAO.GetHeritageByIdAsync(heritageId);
        }

        public IQueryable<Heritage> GetHeritagesQueryable()
        {
            return _entityDAO.GetHeritagesQueryable();
        }

        public async Task<Heritage?> GetHeritageById(long id)
        {
            return await _entityDAO.GetHeritageById(id);
        }

    }
}

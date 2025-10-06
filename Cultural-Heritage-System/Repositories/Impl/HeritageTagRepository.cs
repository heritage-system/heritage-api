using Cultural_Heritage_System.DataAccessObjects;
using Cultural_Heritage_System.Models;
using Cultural_Heritage_System.Repositories;

namespace Cultural_Heritage_System.Repositories.Impl
{
    public class HeritageTagRepository : BaseRepository<HeritageTag>, IHeritageTagRepository
    {
        private readonly HeritageTagDAO _entityDAO;

        public HeritageTagRepository(HeritageTagDAO dao) : base(dao)
        {
            _entityDAO = dao;
        }

        public async Task AddRangeAsync(IEnumerable<HeritageTag> entities)
        {
            await _entityDAO.AddRangeAsync(entities);
        }
    }
}


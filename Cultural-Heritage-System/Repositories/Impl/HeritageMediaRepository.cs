using Cultural_Heritage_System.DataAccessObjects;
using Cultural_Heritage_System.Models;
using Cultural_Heritage_System.Repositories;

namespace Cultural_Heritage_System.Repositories.Impl
{
    public class HeritageMediaRepository : BaseRepository<HeritageMedia>, IHeritageMediaRepository
    {
        private readonly HeritageMediaDAO _entityDAO;

        public HeritageMediaRepository(HeritageMediaDAO dao) : base(dao)
        {
            _entityDAO = dao;
        }

        public async Task AddRangeAsync(IEnumerable<HeritageMedia> entities)
        {
            await _entityDAO.AddRangeAsync(entities);   
        }

    }
}

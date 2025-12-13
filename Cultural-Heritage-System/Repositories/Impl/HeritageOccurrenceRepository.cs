using Cultural_Heritage_System.DataAccessObjects;
using Cultural_Heritage_System.Models;
using Cultural_Heritage_System.Repositories;

namespace Cultural_Heritage_System.Repositories.Impl
{
    public class HeritageOccurrenceRepository : BaseRepository<HeritageOccurrence>, IHeritageOccurrenceRepository
    {
        private readonly HeritageOccurrenceDAO _entityDAO;

        public HeritageOccurrenceRepository(HeritageOccurrenceDAO dao) : base(dao)
        {
            _entityDAO = dao;
        }

        public async Task AddRangeAsync(IEnumerable<HeritageOccurrence> entities)
        {
            await _entityDAO.AddRangeAsync(entities);
        }

        public IQueryable<HeritageOccurrence> QueryOccurrences()
        {
           return _entityDAO.QueryOccurrences();
        }
    }
}

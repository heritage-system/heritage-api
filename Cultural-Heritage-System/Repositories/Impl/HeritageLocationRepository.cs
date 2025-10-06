using Cultural_Heritage_System.DataAccessObjects;
using Cultural_Heritage_System.Models;
using Cultural_Heritage_System.Repositories;

namespace Cultural_Heritage_System.Repositories.Impl
{
    public class HeritageLocationRepository : BaseRepository<HeritageLocation>, IHeritageLocationRepository
    {
        private readonly HeritageLocationDAO _entityDAO;

        public HeritageLocationRepository(HeritageLocationDAO dao) : base(dao)
        {
            _entityDAO = dao;
        }

        public async Task AddRangeAsync(IEnumerable<HeritageLocation> heritageLocations)
        {
            await _entityDAO.AddRangeAsync(heritageLocations);
        }
    }
}

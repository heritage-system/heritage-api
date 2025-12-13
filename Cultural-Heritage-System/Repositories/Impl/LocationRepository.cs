using Cultural_Heritage_System.DataAccessObjects;
using Cultural_Heritage_System.Models;
using Cultural_Heritage_System.Repositories;

namespace Cultural_Heritage_System.Repositories.Impl
{
    public class LocationRepository : BaseRepository<Location>, ILocationRepository
    {
        private readonly LocationDAO _entityDAO;

        public LocationRepository(LocationDAO dao) : base(dao)
        {
            _entityDAO = dao;
        }

        public async Task AddRangeAsync(IEnumerable<Location> heritageLocations)
        {
            await _entityDAO.AddRangeAsync(heritageLocations);
        }
    }
}

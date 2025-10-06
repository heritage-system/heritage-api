using Cultural_Heritage_System.Models;
using Cultural_Heritage_System.Repositories;

namespace Cultural_Heritage_System.Repositories
{
    public interface ILocationRepository : IBaseRepository<Location>
    {
        Task AddRangeAsync(IEnumerable<Location> heritageLocations);
       
    }
}

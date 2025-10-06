using Cultural_Heritage_System.Models;
using Cultural_Heritage_System.Repositories;

namespace Cultural_Heritage_System.Repositories
{
    public interface IHeritageLocationRepository : IBaseRepository<HeritageLocation>
    {
        Task AddRangeAsync(IEnumerable<HeritageLocation> heritageLocations);
        
    }
}

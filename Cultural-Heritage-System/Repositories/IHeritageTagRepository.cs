using Cultural_Heritage_System.Models;
using Cultural_Heritage_System.Repositories;

namespace Cultural_Heritage_System.Repositories
{
    public interface IHeritageTagRepository : IBaseRepository<HeritageTag>
    {
        Task AddRangeAsync(IEnumerable<HeritageTag> entities);      
    }
}


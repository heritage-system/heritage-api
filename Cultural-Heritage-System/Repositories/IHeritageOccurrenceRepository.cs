using Cultural_Heritage_System.Models;
using Cultural_Heritage_System.Repositories;

namespace Cultural_Heritage_System.Repositories
{
    public interface IHeritageOccurrenceRepository
    {
        Task AddRangeAsync(IEnumerable<HeritageOccurrence> entities);
        IQueryable<HeritageOccurrence> QueryOccurrences();
    }
}

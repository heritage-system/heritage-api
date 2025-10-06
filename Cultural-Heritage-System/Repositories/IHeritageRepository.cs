using Cultural_Heritage_System.Models;
using Microsoft.EntityFrameworkCore;

namespace Cultural_Heritage_System.Repositories
{
    public interface IHeritageRepository : IBaseRepository<Heritage>
    {
       Task<IEnumerable<Heritage>> GetAllHeritageAsync();
       IQueryable<Heritage> GetAllQuery();
       Task<bool> HeritageExistsAsync(long heritageId);
       Task<Heritage> GetHeritageByIdAsync(long heritageId);
       IQueryable<Heritage> GetHeritagesQueryable();
       Task<Heritage?> GetHeritageById(long id);

    }
}

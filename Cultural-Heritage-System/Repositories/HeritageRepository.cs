using Cultural_Heritage_System.Models;
using Cultural_Heritage_System.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Cultural_Heritage_System.Repositories
{
    public class HeritageRepository : BaseRepository<Heritage>
    {
        public HeritageRepository(AppDbContext context) : base(context)
        {
        }

        public async Task<Heritage> GetHeritageByIdAsync(long heritageId)
        {
            return await _dbSet
                .Include(h => h.Category)
                .FirstOrDefaultAsync(h => h.Id == heritageId);
        }

        public async Task<bool> HeritageExistsAsync(long heritageId)
        {
            return await _dbSet.AnyAsync(h => h.Id == heritageId);
        }
    }
}
using Cultural_Heritage_System.Models;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace Cultural_Heritage_System.DataAccessObjects
{
    public class PointHistoryDAO : BaseDAO<PointHistory>
    {
        public PointHistoryDAO(AppDbContext dbContext) : base(dbContext)
        {
        }
      
        public async Task<PointHistory?> GetByIdAsync(int packageId)
        {
            return await _dbSet                
                .FirstOrDefaultAsync(p => p.Id == packageId);
        }

        public IQueryable<PointHistory> GetQueryable()
        {
            return _dbSet               
                .AsQueryable();
        }

        public async Task<IEnumerable<PointHistory>> GetListByUserId(int userId)
        {
            return await _dbSet
                .Where(p => p.UserId == userId)             
                .ToListAsync();                  
        }
    }

}

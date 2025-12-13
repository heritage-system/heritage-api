using Cultural_Heritage_System.Models;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace Cultural_Heritage_System.DataAccessObjects
{
    public class GameMatchHistoryDAO : BaseDAO<GameMatchHistory>
    {
        public GameMatchHistoryDAO(AppDbContext dbContext) : base(dbContext)
        {
        }
      
        public async Task<GameMatchHistory?> GetByIdAsync(int packageId)
        {
            return await _dbSet                
                .FirstOrDefaultAsync(p => p.Id == packageId);
        }

        public IQueryable<GameMatchHistory> GetQueryable()
        {
            return _dbSet               
                .AsQueryable();
        }

        public async Task<IEnumerable<GameMatchHistory>> GetListByUserId(int userId)
        {
            return await _dbSet
                .Where(p => p.Player1Id == userId || p.Player2Id == userId)             
                .ToListAsync();                  
        }
    }

}

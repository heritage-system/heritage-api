using Cultural_Heritage_System.Models;
using Cultural_Heritage_System.Repositories;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Cultural_Heritage_System.DataAccessObjects
{
    public class ProfileDAO : BaseDAO<Profile>
    {
        public ProfileDAO(AppDbContext dbContext) : base(dbContext)
        {
        }

        public async Task<Profile?> GetProfileByUserIdAsync(long userId)
        {
            return await _dbSet.FirstOrDefaultAsync(p => p.UserId == userId);
        }

        public IQueryable<Profile> GetQueryable()
        {
            return _context.Profiles.AsQueryable();
        }
    }
}

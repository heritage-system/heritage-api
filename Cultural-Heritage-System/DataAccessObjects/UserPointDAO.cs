using Cultural_Heritage_System.Models;
using Cultural_Heritage_System.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Cultural_Heritage_System.DataAccessObjects
{
    public class UserPointDAO : BaseDAO<UserPoint>
    {
        private readonly ILogger<UserPointDAO> _logger;

        public UserPointDAO(AppDbContext context, ILogger<UserPointDAO> logger)
            : base(context)
        {
            _logger = logger;
        }

        public async Task<UserPoint?> GetUserPointById(int id)
        {
            return await _dbSet
                .Include(c => c.User)
                    .ThenInclude(u => u.Profile)                   
                .FirstOrDefaultAsync(c => c.Id == id);
        }

        public IQueryable<UserPoint> GetUserPointsQueryable()
        {
            return _dbSet
                .Include(c => c.User)
                    .ThenInclude(u => u.Profile);               
        }

        public async Task<UserPoint?> GetUserPointByUserId(int id)
        {
            return await _dbSet
                .Include(c => c.User)
                    .ThenInclude(u => u.Profile)                   
                .FirstOrDefaultAsync(c => c.UserId == id);
        }
    }
}
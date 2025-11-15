using Cultural_Heritage_System.Common;
using Cultural_Heritage_System.Models;
using Cultural_Heritage_System.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Cultural_Heritage_System.DataAccessObjects
{
    public class UserDAO : BaseDAO<User>
    {
        private readonly ILogger<UserDAO> _logger;

        public UserDAO(AppDbContext context, ILogger<UserDAO> logger)
            : base(context)
        {
            _logger = logger;
        }

        public async Task<User?> FindUserByEmailOrUserName(string emailOrUserName)
        {
            return await _context.Users
                .Include(u => u.Role)
                .Include(u => u.Profile)
                .FirstOrDefaultAsync(u => u.Email == emailOrUserName || u.UserName == emailOrUserName);
        }

        public async Task<User?> FindUserByEmail(string email)
        {
            return await _dbSet
                .Include(u => u.Role)
                .Include(u => u.Profile)
                .FirstOrDefaultAsync(u => u.Email == email);
        }

        public async Task<List<User>> GetAllUsers(int page, int size)
        {
            return await _dbSet
                .Skip((page - 1) * size)
                .Take(size)
                .ToListAsync();
        }


        public async Task<int> GetTotalUsersCount()
        {
            return await _dbSet.CountAsync();
        }

        public async Task<User?> FindUserById(long id)
        {
            return await _dbSet
                .Include(u => u.Role)
                .Include(u => u.Profile)
                .FirstOrDefaultAsync(u => u.Id == id);
        }

        public IQueryable<User> GetQueryable()
        {
            return _dbSet
                .Include(u => u.Role)
                .Include(u => u.Profile)
                .Where(u => u.Role.Name == DefinitionRole.MEMBER);
        }

        public async Task<User?> FindUserByIdWithAllRelations(int id)
        {
            return await _dbSet
                // --- 1:1 (Reference navigations)
                .Include(u => u.Role)
                .Include(u => u.Profile)            

                // --- 1:n (Collection navigations)
                .Include(u => u.Favorites)
                .Include(u => u.Reviews)
                .Include(u => u.Reports)             
                .Include(u => u.Notifications)
                .Include(u => u.ReviewLikes)
                .Include(u => u.ReviewReports)               
                .Include(u => u.Subscriptions)
                .Include(u => u.ContributionAccessLogs)          
                .Include(u => u.ContributionSaves)
                .Include(u => u.ContributionReviews)              
                .Include(u => u.ContributionReports)
                
                .FirstOrDefaultAsync(u => u.Id == id);
        }

    }
}

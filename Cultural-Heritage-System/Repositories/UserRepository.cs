using Cultural_Heritage_System.Models;
using medical_appointment_booking.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Cultural_Heritage_System.Repositories
{
    public class UserRepository : BaseRepository<User>
    {
        private readonly ILogger<UserRepository> _logger;

        public UserRepository(AppDbContext context, ILogger<UserRepository> logger)
            : base(context)
        {
            _logger = logger;
        }

        public async Task<User?> FindUserByEmail(string email)
        {
            return await _dbSet
                .Include(u => u.Role)
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
                .FirstOrDefaultAsync(u => u.Id == id);
        }
    
    }
}

using Cultural_Heritage_System.Common;
using Cultural_Heritage_System.Models;
using Cultural_Heritage_System.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Cultural_Heritage_System.DataAccessObjects
{
    public class StaffDAO : BaseDAO<Staff>
    {
        private readonly ILogger<StaffDAO> _logger;

        public StaffDAO(AppDbContext context, ILogger<StaffDAO> logger)
            : base(context)
        {
            _logger = logger;
        }

        public async Task<Staff?> GetStaffById(int id)
        {
            return await _dbSet
                .Include(c => c.User)
                .ThenInclude(u => u.Profile)                  
                .FirstOrDefaultAsync(c => c.Id == id);
        }

        public IQueryable<Staff> GetStaffsQueryable()
        {
            return _dbSet
                .Include(c => c.User)
                    .ThenInclude(u => u.Profile)  
                .Include(c => c.ContributionAcceptances);
        }

        public async Task<Staff?> GetStaffByUserId(int id)
        {
            return await _dbSet
                .Include(c => c.User)
                .ThenInclude(u => u.Profile)   
                .Include(c => c.ContributionAcceptances)
                .FirstOrDefaultAsync(c => c.UserId == id);
        }

        public async Task<List<Staff>> GetActiveReviewersAsync()
        {
            return await _dbSet
                .Where(s => s.StaffStatus == StaffStatus.ACTIVE
                         && s.StaffRole == StaffRole.CONTENT_REVIEWER)
                .Include(s => s.ContributionAcceptances)
                .ToListAsync();
        }

        // Lấy id staff gần nhất đã được assign
        public async Task<int?> GetLastAssignedStaffIdAsync()
        {
            return await _context.SystemLogs
                .Where(l => l.Action == SystemLogAction.STAFF_ASSIGNED_CONTRIBUTION)
                .OrderByDescending(l => l.CreatedAt)
                .Select(l => (int?)l.UserId) 
                .FirstOrDefaultAsync();
        }

        public async Task<Staff?> GetByUserIdAsync(int userId)
        {
            return await _dbSet
                .Include(s => s.User)
                .FirstOrDefaultAsync(s => s.UserId == userId);
        }

    }
}
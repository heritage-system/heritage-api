using Cultural_Heritage_System.Models;
using medical_appointment_booking.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Cultural_Heritage_System.Repositories
{
    public class ReportRepository : BaseRepository<Report>
    {
        private readonly ILogger<ReportRepository> _logger;

        public ReportRepository(AppDbContext context, ILogger<ReportRepository> logger)
            : base(context)
        {
            _logger = logger;
        }

        public override async Task<IEnumerable<Report>> GetAllAsync()
        {
            return await _dbSet
                .Include(r => r.User)
                .Include(r => r.Heritage)
                .ToListAsync();
        }

        public override async Task<Report> GetByIdAsync(object id)
        {
            var reportId = id is long l ? l : Convert.ToInt64(id);
            return await _dbSet
                .Include(r => r.User)
                .Include(r => r.Heritage)
                .FirstOrDefaultAsync(r => r.Id == reportId);
        }

        public IQueryable<Report> GetAllQuery()
        {
            return _dbSet
                .Include(r => r.User)
                .Include(r => r.Heritage).AsQueryable();
        }
    }
}

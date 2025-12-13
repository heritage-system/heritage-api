using Cultural_Heritage_System.Models;
using Cultural_Heritage_System.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Cultural_Heritage_System.DataAccessObjects
{
    public class ContributionReportDAO : BaseDAO<ContributionReport>
    {
        private readonly ILogger<ContributionReportDAO> _logger;

        public ContributionReportDAO(AppDbContext context, ILogger<ContributionReportDAO> logger)
            : base(context)
        {
            _logger = logger;
        }

        public override async Task<IEnumerable<ContributionReport>> GetAllAsync()
        {
            return await _dbSet
                .Include(r => r.User)
                .Include(r => r.Contribution)
                .ToListAsync();
        }

        public async Task<ContributionReport> GetByIdAsync(long id)
        {          
            return await _dbSet
                .Include(r => r.User)
                .Include(r => r.Contribution)
                .FirstOrDefaultAsync(r => r.Id == id);
        }

        public IQueryable<ContributionReport> GetAllQuery()
        {
            return _dbSet
                .Include(r => r.User)
                .Include(r => r.Contribution).AsQueryable();
        }
    }
}

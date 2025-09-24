using Cultural_Heritage_System.Models;
using Cultural_Heritage_System.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Cultural_Heritage_System.Repositories
{
    public class ContributionRepository : BaseRepository<Contribution>
    {
        private readonly ILogger<ContributionRepository> _logger;

        public ContributionRepository(AppDbContext context, ILogger<ContributionRepository> logger)
            : base(context)
        {
            _logger = logger;
        }

        public IQueryable<Contribution> GetContributionsQueryable()
        {
            return _context.Contributions
                .Include(h => h.Contributor)
                .ThenInclude(c => c.User)
                .ThenInclude(u => u.Profile)
                .Include(h => h.ContributionHeritageTags)
                .Include(h => h.ContributionAccessLogs)
                .AsQueryable();
        }

        public async Task<Contribution?> GetContributionById(long id)
        {
            return await _dbSet
               .Include(h => h.Contributor)
                .ThenInclude(c => c.User)
                .ThenInclude(u => u.Profile)
               .Include(h => h.ContributionHeritageTags)
                .ThenInclude(c => c.Heritage)
               .Include(h => h.ContributionAccessLogs)
               .Include(c => c.ContributionSaves)
                .FirstOrDefaultAsync(u => u.Id == id);
        }
    }
}
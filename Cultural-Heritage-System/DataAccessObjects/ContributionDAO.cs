using Cultural_Heritage_System.Common;
using Cultural_Heritage_System.Dtos.Models;
using Cultural_Heritage_System.Models;
using Cultural_Heritage_System.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Cultural_Heritage_System.DataAccessObjects
{
    public class ContributionDAO : BaseDAO<Contribution>
    {
        private readonly ILogger<ContributionDAO> _logger;

        public ContributionDAO(AppDbContext context, ILogger<ContributionDAO> logger)
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

        public IQueryable<Contribution> GetApprovedContributionsQueryable()
        {
            return _context.Contributions
                .Include(h => h.Contributor)
                .ThenInclude(c => c.User)
                .ThenInclude(u => u.Profile)
                .Include(h => h.ContributionHeritageTags)
                .Include(h => h.ContributionAccessLogs)
                .Where(c => c.Status == ContributionStatus.APPROVED)
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
               .Include(c => c.Reviews)
               .Include(c => c.ContributionReports)
               .Include(c => c.ContributionAcceptances)
                .AsSplitQuery()
                .FirstOrDefaultAsync(u => u.Id == id);
        }

        public async Task<Contribution?> GetContributionByIdAndStatus(long id,ContributionStatus contributionStatus)
        {
            return await _dbSet
               .Include(h => h.Contributor)
                .ThenInclude(c => c.User)
                .ThenInclude(u => u.Profile)
               .Include(h => h.ContributionHeritageTags)
                .ThenInclude(c => c.Heritage)
               .Include(h => h.ContributionAccessLogs)
               .Include(c => c.ContributionSaves)
               .Include(c => c.Reviews)
               .Include(c => c.ContributionReports)
               .Where(c => c.Status == contributionStatus)
                .FirstOrDefaultAsync(u => u.Id == id);
        }

        public async Task<List<TrendingContributorDto>> GetTopContributorsAsync()
        {
            var topContributors = await _dbSet
                .Where(c => c.Status == ContributionStatus.APPROVED)
                .GroupBy(c => new
                {
                    c.ContributorId,
                    c.Contributor.User.UserName,
                    c.Contributor.User.Profile.AvatarUrl 
                })
                .Select(g => new TrendingContributorDto
                {
                    ContributorId = g.Key.ContributorId,
                    ContributorName = g.Key.UserName,
                    AvatarUrl = g.Key.AvatarUrl,
                    TotalPosts = g.Count(),
                    TotalViews = g.Sum(c => c.ContributionAccessLogs.Count),
                    TotalComments = g.Sum(c => c.Reviews.Count),
                    TotalSaves = g.Sum(c => c.ContributionSaves.Count),
                    Score =
                        g.Sum(c => c.ContributionAccessLogs.Count) * 0.2 +
                        g.Sum(c => c.Reviews.Count) * 0.5 +
                        g.Sum(c => c.ContributionSaves.Count) * 1.0
                })
                .OrderByDescending(x => x.Score)
                .Take(5)
                .ToListAsync();

            return topContributors;
        }

        public IQueryable<Contribution> GetContributionsByContributorIdQueryable(int contributorId)
        {
            return _context.Contributions
                .Include(h => h.Contributor)
                .ThenInclude(c => c.User)
                .ThenInclude(u => u.Profile)
                .Include(h => h.ContributionHeritageTags)
                .Include(h => h.ContributionAccessLogs)
                .Where(c => c.ContributorId == contributorId)
                .AsQueryable();
        }

        public IQueryable<Contribution> GetContributionsByStaffIdQueryable(int staffId)
        {
            return _dbSet
                .Include(c => c.ContributionHeritageTags)
                    .ThenInclude(ht => ht.Heritage)
                .Include(c => c.Contributor)
                    .ThenInclude(con => con.User)
                        .ThenInclude(u => u.Profile)
                .Include(c => c.ContributionAccessLogs)
                .AsSplitQuery()
                .Where(c => c.ContributionAcceptances.Any(a => a.StaffId == staffId));
        }

        public Task<Contribution?> GetContributionForStaffAsync(long contributionId, int staffId)
        {
            return _dbSet
                .Include(c => c.ContributionHeritageTags)
                    .ThenInclude(ht => ht.Heritage)
                .Include(c => c.Contributor)
                    .ThenInclude(con => con.User)
                        .ThenInclude(u => u.Profile)
                .Include(c => c.ContributionAccessLogs)
                .Include(c => c.ContributionAcceptances)
                .AsSplitQuery()
                .FirstOrDefaultAsync(c => c.Id == contributionId);
        }

    }
}
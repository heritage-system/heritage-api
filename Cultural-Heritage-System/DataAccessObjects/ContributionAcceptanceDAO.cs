using Cultural_Heritage_System.Models;
using Microsoft.EntityFrameworkCore;

namespace Cultural_Heritage_System.DataAccessObjects
{
    public class ContributionAcceptanceDAO : BaseDAO<ContributionAcceptance>
    {
    public ContributionAcceptanceDAO(AppDbContext context) : base(context) { }
        public IQueryable<ContributionAcceptance> GetQueryable()
        {
            return _dbSet
                .Include(a => a.Contribution)
                    .ThenInclude(c => c.ContributionHeritageTags)
                        .ThenInclude(ht => ht.Heritage)
                .Include(a => a.Contribution)
                    .ThenInclude(c => c.Contributor)
                        .ThenInclude(con => con.User)
                            .ThenInclude(u => u.Profile)
                .Include(a => a.Staff)
                    .ThenInclude(s => s.User)
                .AsSplitQuery();
        }

        public Task<List<ContributionAcceptance>> GetByStaffIdAsync(int staffId)
            => GetQueryable()
                .Where(a => a.StaffId == staffId)
                .ToListAsync();

        public Task<ContributionAcceptance?> GetByIdAsync(long id)
            => _dbSet
                .Include(a => a.Contribution)
                .FirstOrDefaultAsync(a => a.Id == id);

        public Task<ContributionAcceptance?> GetByContributionAndStaffIdAsync(long contributionId, int staffId)
            => GetQueryable().FirstOrDefaultAsync(a => a.ContributionId == contributionId && a.StaffId == staffId);

        public Task<List<ContributionAcceptance>> GetAllByContributionIdAsync(long contributionId)
            => _dbSet
                .Where(a => a.ContributionId == contributionId)
                .Select(a => new ContributionAcceptance { Id = a.Id, Status = a.Status })
                .ToListAsync();

        public Task<bool> IsAssignedToStaffAsync(long contributionId, int staffId)
            => _dbSet.AnyAsync(a => a.ContributionId == contributionId && a.StaffId == staffId);

    }
}

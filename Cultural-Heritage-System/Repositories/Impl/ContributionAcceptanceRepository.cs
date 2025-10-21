using Cultural_Heritage_System.DataAccessObjects;
using Cultural_Heritage_System.Models;

namespace Cultural_Heritage_System.Repositories.Impl
{
    public class ContributionAcceptanceRepository : BaseRepository<ContributionAcceptance>, IContributionAcceptanceRepository
    {
        private readonly ContributionAcceptanceDAO _dao;

        public ContributionAcceptanceRepository(ContributionAcceptanceDAO dao) : base(dao)
        {
            _dao = dao;
        }

        public IQueryable<ContributionAcceptance> GetQueryable() => _dao.GetQueryable();

        public Task<List<ContributionAcceptance>> GetByStaffIdAsync(int staffId)
            => _dao.GetByStaffIdAsync(staffId);

        public Task<ContributionAcceptance?> GetByIdAsync(long id)
            => _dao.GetByIdAsync(id);

        public Task<ContributionAcceptance?> GetByContributionAndStaffIdAsync(long contributionId, int staffId)
            => _dao.GetByContributionAndStaffIdAsync(contributionId, staffId);

        public Task<List<ContributionAcceptance>> GetAllByContributionIdAsync(long contributionId)
            => _dao.GetAllByContributionIdAsync(contributionId);

        public Task<bool> IsAssignedToStaffAsync(long contributionId, int staffId)
            => _dao.IsAssignedToStaffAsync(contributionId, staffId);

    }
}

using Cultural_Heritage_System.Common;
using Cultural_Heritage_System.Models;

namespace Cultural_Heritage_System.Repositories
{
    public interface IContributionAcceptanceRepository : IBaseRepository<ContributionAcceptance>
    {
        IQueryable<ContributionAcceptance> GetQueryable();
        Task<List<ContributionAcceptance>> GetByStaffIdAsync(int staffId);
        Task<ContributionAcceptance?> GetByIdAsync(long id);
        Task<ContributionAcceptance?> GetByContributionAndStaffIdAsync(long contributionId, int staffId);
        Task<List<ContributionAcceptance>> GetAllByContributionIdAsync(long contributionId);
        Task<bool> IsAssignedToStaffAsync(long contributionId, int staffId);

    }
}   
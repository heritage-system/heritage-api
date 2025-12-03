using Cultural_Heritage_System.Models;
using Cultural_Heritage_System.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Cultural_Heritage_System.Repositories
{
    public interface IContributionUnlockRepository : IBaseRepository<ContributionUnlock>
    {
        Task<ContributionUnlock> GetContributionUnlockByUserAndContribution(int userId, long contributionId);
        IQueryable<ContributionUnlock> GetContributionUnlocksQueryByUserId(int userId);
        Task<bool> IsContributionUnlockExists(int userId, long contributionId);
        Task<int> GetContributionUnlockCountByUserId(int userId);
        
    }
}

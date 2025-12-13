using Cultural_Heritage_System.Models;
using Cultural_Heritage_System.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Cultural_Heritage_System.Repositories
{
    public interface IContributionSaveRepository : IBaseRepository<ContributionSave>
    {
        Task<ContributionSave> GetContributionSaveByUserAndContribution(int userId, long contributionId);
        IQueryable<ContributionSave> GetContributionSavesQueryByUserId(int userId);
        Task<bool> IsContributionSaveExists(int userId, long contributionId);
        Task<int> GetContributionSaveCountByUserId(int userId);
        
    }
}

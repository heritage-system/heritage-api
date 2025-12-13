using Cultural_Heritage_System.DataAccessObjects;
using Cultural_Heritage_System.Models;
using Cultural_Heritage_System.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Cultural_Heritage_System.Repositories.Impl
{
    public class ContributionUnlockRepository : BaseRepository<ContributionUnlock>, IContributionUnlockRepository
    {
        private readonly ContributionUnlockDAO _entityDAO;

        public ContributionUnlockRepository(ContributionUnlockDAO dao) : base(dao)
        {
            _entityDAO = dao;
        }

        public async Task<ContributionUnlock> GetContributionUnlockByUserAndContribution(int userId, long contributionId)
        {
            return await _entityDAO.GetContributionUnlockByUserAndContribution(userId,contributionId);
        }

 

        public IQueryable<ContributionUnlock> GetContributionUnlocksQueryByUserId(int userId)
        {
            return _entityDAO.GetContributionUnlocksQueryByUserId(userId);
        }

        public async Task<bool> IsContributionUnlockExists(int userId, long contributionId)
        {
            return await _entityDAO.IsContributionUnlockExists(userId,contributionId);
        }

        public async Task<int> GetContributionUnlockCountByUserId(int userId)
        {
            return await _entityDAO.GetContributionUnlockCountByUserId(userId);
        }
    }
}

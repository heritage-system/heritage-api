using Cultural_Heritage_System.DataAccessObjects;
using Cultural_Heritage_System.Models;
using Cultural_Heritage_System.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Cultural_Heritage_System.Repositories.Impl
{
    public class ContributionSaveRepository : BaseRepository<ContributionSave>, IContributionSaveRepository
    {
        private readonly ContributionSaveDAO _entityDAO;

        public ContributionSaveRepository(ContributionSaveDAO dao) : base(dao)
        {
            _entityDAO = dao;
        }

        public async Task<ContributionSave> GetContributionSaveByUserAndContribution(int userId, long contributionId)
        {
            return await _entityDAO.GetContributionSaveByUserAndContribution(userId,contributionId);
        }

 

        public IQueryable<ContributionSave> GetContributionSavesQueryByUserId(int userId)
        {
            return _entityDAO.GetContributionSavesQueryByUserId(userId);
        }

        public async Task<bool> IsContributionSaveExists(int userId, long contributionId)
        {
            return await _entityDAO.IsContributionSaveExists(userId,contributionId);
        }

        public async Task<int> GetContributionSaveCountByUserId(int userId)
        {
            return await _entityDAO.GetContributionSaveCountByUserId(userId);
        }
    }
}

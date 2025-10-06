using Cultural_Heritage_System.DataAccessObjects;
using Cultural_Heritage_System.Models;
using Cultural_Heritage_System.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Cultural_Heritage_System.Repositories.Impl
{
    public class ContributionAccessLogRepository : BaseRepository<ContributionAccessLog>, IContributionAccessLogRepository
    {
        private readonly ContributionAccessLogDAO _entityDAO;

        public ContributionAccessLogRepository(ContributionAccessLogDAO dao) : base(dao)
        {
            _entityDAO = dao;
        }

        public async Task<ContributionAccessLog?> GetContributionAccessLogs(int userId, int contributionId)
        {
            return await _entityDAO.GetContributionAccessLogs(userId, contributionId);
        }
    }
}

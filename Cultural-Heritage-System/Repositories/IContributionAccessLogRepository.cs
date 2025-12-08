using Cultural_Heritage_System.Models;
using Cultural_Heritage_System.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Cultural_Heritage_System.Repositories
{
    public interface IContributionAccessLogRepository : IBaseRepository<ContributionAccessLog>
    {
        IQueryable<ContributionAccessLog> GetContributionAccessLogsQueryable();
        Task<ContributionAccessLog?> GetContributionAccessLogs(int userId, int contributionId);       
    }
}

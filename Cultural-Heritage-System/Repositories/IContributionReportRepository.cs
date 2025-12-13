using Cultural_Heritage_System.Models;
using Cultural_Heritage_System.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Cultural_Heritage_System.Repositories
{
    public interface IContributionReportRepository : IBaseRepository<ContributionReport>
    {

        Task<IEnumerable<ContributionReport>> GetAllReportAsync();


        Task<ContributionReport> GetByIdAsync(long id);


        IQueryable<ContributionReport> GetAllQuery();
        
    }
}

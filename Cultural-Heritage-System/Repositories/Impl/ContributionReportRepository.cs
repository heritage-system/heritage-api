using Cultural_Heritage_System.DataAccessObjects;
using Cultural_Heritage_System.Models;
using Cultural_Heritage_System.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Cultural_Heritage_System.Repositories.Impl
{
    public class ContributionReportRepository : BaseRepository<ContributionReport>, IContributionReportRepository
    {
        private readonly ContributionReportDAO _entityDAO;

        public ContributionReportRepository(ContributionReportDAO dao) : base(dao)
        {
            _entityDAO = dao;
        }

        public async Task<IEnumerable<ContributionReport>> GetAllReportAsync()
        {
            return await _entityDAO.GetAllAsync();
        }

        public async Task<ContributionReport> GetByIdAsync(long id)
        {          
            return await _entityDAO.GetByIdAsync(id);
        }

        public IQueryable<ContributionReport> GetAllQuery()
        {
            return _entityDAO.GetAllQuery();
        }
    }
}

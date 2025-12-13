using Cultural_Heritage_System.DataAccessObjects;
using Cultural_Heritage_System.Models;
using Cultural_Heritage_System.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Cultural_Heritage_System.Repositories.Impl
{
    public class ReportRepository : BaseRepository<Report>, IReportRepository
    {
        private readonly ReportDAO _entityDAO;

        public ReportRepository(ReportDAO dao) : base(dao)
        {
            _entityDAO = dao;
        }

        public async Task<IEnumerable<Report>> GetAllReportAsync()
        {
            return await _entityDAO.GetAllAsync();
        }

        public async Task<Report> GetReportByIdAsync(object id)
        {          
            return await _entityDAO.GetByIdAsync(id);
        }

        public IQueryable<Report> GetAllQuery()
        {
            return _entityDAO.GetAllQuery();
        }
    }
}

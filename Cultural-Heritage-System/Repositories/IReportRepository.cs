using Cultural_Heritage_System.Models;
using Cultural_Heritage_System.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Cultural_Heritage_System.Repositories
{
    public interface IReportRepository : IBaseRepository<Report>
    {
        Task<IEnumerable<Report>> GetAllReportAsync();

        Task<Report> GetReportByIdAsync(object id);
        IQueryable<Report> GetAllQuery();
       
    }
}

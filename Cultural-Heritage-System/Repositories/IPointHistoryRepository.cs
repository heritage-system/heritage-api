using Cultural_Heritage_System.Models;

namespace Cultural_Heritage_System.Repositories
{
    public interface IPointHistoryRepository : IBaseRepository<PointHistory>
    {
        IQueryable<PointHistory> GetQueryable();    
        Task<PointHistory?> GetByIdAsync(int id);
        Task<IEnumerable<PointHistory>> GetListByUserId(int userId);
    }
}

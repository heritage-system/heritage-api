using Cultural_Heritage_System.Models;

namespace Cultural_Heritage_System.Repositories
{
    public interface IGameMatchHistoryRepository : IBaseRepository<GameMatchHistory>
    {
        IQueryable<GameMatchHistory> GetQueryable();    
        Task<GameMatchHistory?> GetByIdAsync(int id);
        Task<IEnumerable<GameMatchHistory>> GetListByUserId(int userId);
    }
}

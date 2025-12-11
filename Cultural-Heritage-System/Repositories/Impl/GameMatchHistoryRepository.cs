using Cultural_Heritage_System.DataAccessObjects;
using Cultural_Heritage_System.Models;

namespace Cultural_Heritage_System.Repositories.Impl
{
    public class GameMatchHistoryRepository : BaseRepository<GameMatchHistory>, IGameMatchHistoryRepository
    {
        private readonly GameMatchHistoryDAO _dao;

        public GameMatchHistoryRepository(GameMatchHistoryDAO dao) : base(dao)
        {
            _dao = dao;
        }

   
        public async Task<GameMatchHistory?> GetByIdAsync(int id)
        {
            return await _dao.GetByIdAsync(id);
        }

        public IQueryable<GameMatchHistory> GetQueryable()
        {
            return _dao.GetQueryable();
        }

        public async Task<IEnumerable<GameMatchHistory>> GetListByUserId(int userId)
        {
            return await _dao.GetListByUserId(userId);
        }
    }
}

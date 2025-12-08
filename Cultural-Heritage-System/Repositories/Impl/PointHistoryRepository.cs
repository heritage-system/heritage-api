using Cultural_Heritage_System.DataAccessObjects;
using Cultural_Heritage_System.Models;

namespace Cultural_Heritage_System.Repositories.Impl
{
    public class PointHistoryRepository : BaseRepository<PointHistory>, IPointHistoryRepository
    {
        private readonly PointHistoryDAO _dao;

        public PointHistoryRepository(PointHistoryDAO dao) : base(dao)
        {
            _dao = dao;
        }

   
        public async Task<PointHistory?> GetByIdAsync(int id)
        {
            return await _dao.GetByIdAsync(id);
        }

        public IQueryable<PointHistory> GetQueryable()
        {
            return _dao.GetQueryable();
        }

        public async Task<IEnumerable<PointHistory>> GetListByUserId(int userId)
        {
            return await _dao.GetListByUserId(userId);
        }
    }
}

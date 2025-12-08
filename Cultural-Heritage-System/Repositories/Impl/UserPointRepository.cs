using Cultural_Heritage_System.DataAccessObjects;
using Cultural_Heritage_System.Models;
using Cultural_Heritage_System.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Cultural_Heritage_System.Repositories.Impl
{
    public class UserPointRepository : BaseRepository<UserPoint>, IUserPointRepository
    {
        private readonly  UserPointDAO _entityDAO;

        public  UserPointRepository(UserPointDAO dao) : base(dao)
        {
            _entityDAO = dao;
        }

        public async Task<UserPoint?> GetUserPointById(int id)
        {
            return await _entityDAO.GetUserPointById(id);
        }

        public IQueryable<UserPoint> GetUserPointsQueryable()
        {
            return _entityDAO.GetUserPointsQueryable();
        }

        public async Task<UserPoint?> GetUserPointByUserId(int id)
        {
            return await _entityDAO.GetUserPointByUserId(id);
        }
    }
}
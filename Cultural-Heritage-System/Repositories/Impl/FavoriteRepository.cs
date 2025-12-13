using Cultural_Heritage_System.DataAccessObjects;
using Cultural_Heritage_System.Models;
using Cultural_Heritage_System.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Cultural_Heritage_System.Repositories.Impl
{
    public class FavoriteRepository : BaseRepository<Favorite>, IFavoriteRepository
    {
        private readonly FavoriteDAO _entityDAO;

        public FavoriteRepository(FavoriteDAO dao) : base(dao)
        {
            _entityDAO = dao;
        }
      

        public IQueryable<Favorite> GetFavoritesQueryByUserId(int userId)
        {
            return _entityDAO.GetFavoritesQueryByUserId(userId);
        }

        public async Task<bool> IsFavoriteExistsAsync(int userId, long heritageId)
        {
            return await _entityDAO.IsFavoriteExistsAsync(userId,heritageId);
        }

        public async Task<int> GetFavoriteCountByUserIdAsync(int userId)
        {
            return await _entityDAO.GetFavoriteCountByUserIdAsync(userId);
        }

        public Task<Favorite> GetFavoriteByUserAndHeritageAsync(int userId, long heritageId)
        {
            return _entityDAO.GetFavoriteByUserAndHeritageAsync(userId, heritageId);
        }

        public IQueryable<Favorite> GetQueryable()
        {
            return _entityDAO.GetQueryable();
        }
    }
}
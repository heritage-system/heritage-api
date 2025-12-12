using Cultural_Heritage_System.Models;
using Cultural_Heritage_System.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Cultural_Heritage_System.DataAccessObjects
{
    public class FavoriteDAO : BaseDAO<Favorite>
    {
        public FavoriteDAO(AppDbContext context) : base(context)
        {
        }

        public async Task<Favorite> GetFavoriteByUserAndHeritageAsync(int userId, long heritageId)
        {
            return await _dbSet
                .FirstOrDefaultAsync(f => f.UserId == userId && f.HeritageId == heritageId);
        }

        //public async Task<List<Favorite>> GetFavoritesByUserIdAsync(int userId)
        //{
        //    return await _dbSet
        //        .Include(f => f.Heritage)
        //        .ThenInclude(h => h.Category)
        //        .Where(f => f.UserId == userId)
        //        .OrderByDescending(f => f.CreatedAt)
        //        .ToListAsync();
        //}

        public IQueryable<Favorite> GetFavoritesQueryByUserId(int userId)
        {
            return _dbSet
                .Include(f => f.Heritage)
                .ThenInclude(h => h.Category)
                .Where(f => f.UserId == userId)
                .OrderByDescending(f => f.CreatedAt);
        }

        public async Task<bool> IsFavoriteExistsAsync(int userId, long heritageId)
        {
            return await _dbSet
                .AnyAsync(f => f.UserId == userId && f.HeritageId == heritageId);
        }

        public async Task<int> GetFavoriteCountByUserIdAsync(int userId)
        {
            return await _dbSet
                .CountAsync(f => f.UserId == userId);
        }

        public IQueryable<Favorite> GetQueryable()
        {
            return _dbSet
                .Include(f => f.Heritage)
                    .ThenInclude(h => h.Category);
        }
    }
}
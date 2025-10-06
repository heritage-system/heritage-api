using Cultural_Heritage_System.Models;
using Cultural_Heritage_System.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Cultural_Heritage_System.Repositories
{
    public interface IFavoriteRepository : IBaseRepository<Favorite>
    {
        Task<Favorite> GetFavoriteByUserAndHeritageAsync(int userId, long heritageId);
        IQueryable<Favorite> GetFavoritesQueryByUserId(int userId);
        Task<bool> IsFavoriteExistsAsync(int userId, long heritageId);
        Task<int> GetFavoriteCountByUserIdAsync(int userId);
      
    }
}
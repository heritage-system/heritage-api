using Cultural_Heritage_System.Dtos.Request;
using Cultural_Heritage_System.Dtos.Response;

namespace Cultural_Heritage_System.Services
{
    public interface IFavoriteService
    {
        //Task<FavoriteListResponse> GetFavoritesByUserIdAsync(int userId);
        Task <PageResponse<FavoriteHeritageResponse>> GetFavoritesByUserIdAsync(int userId, int page, int pageSize);
        Task AddFavoriteAsync(int userId, AddFavoriteRequest request);
        Task RemoveFavoriteAsync(int userId, RemoveFavoriteRequest request);
    }
}
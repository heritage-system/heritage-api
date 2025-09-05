using Cultural_Heritage_System.Dtos.Request;
using Cultural_Heritage_System.Dtos.Response;

namespace Cultural_Heritage_System.Services
{
    public interface IFavoriteService
    {
        //Task<FavoriteListResponse> GetFavoritesByUserIdAsync(int userId);
        //Task <PageResponse<FavoriteHeritageResponse>> GetFavoritesByUserIdAsync(int userId, int page, int pageSize);
        Task<PageResponse<FavoriteHeritageResponse>> GetFavoritesAsync (int page,  int pageSize, string? searchName);
        Task AddFavoriteAsync(AddFavoriteRequest request);
        Task RemoveFavoriteAsync(RemoveFavoriteRequest request);
    }
}
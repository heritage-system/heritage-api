using Cultural_Heritage_System.Dtos.Request;
using Cultural_Heritage_System.Dtos.Response;
using Cultural_Heritage_System.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;

namespace Cultural_Heritage_System.Controllers
{
    [Route("api/v1/favorites")]
    [ApiController]
    [Authorize]
    public class FavoriteController : ControllerBase
    {
        private readonly IFavoriteService favoriteService;

        public FavoriteController(IFavoriteService favoriteService)
        {
            this.favoriteService = favoriteService;
        }

        [HttpGet]
        public async Task<ApiResponse<PageResponse<FavoriteHeritageResponse>>> GetFavorites(
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10,
            [FromQuery] string? searchName = null)
        {
            try
            {
                // Validate pagination parameters
                if (page < 1) page = 1;
                if (pageSize < 1 || pageSize > 100) pageSize = 10;

                var favorites = await favoriteService.GetFavoritesAsync(page, pageSize, searchName);

                return new ApiResponse<PageResponse<FavoriteHeritageResponse>>(
                    code: 200,
                    message: "Get favorites successfully",
                    result: favorites
                );
            }
            catch (Exception ex)
            {
                return new ApiResponse<PageResponse<FavoriteHeritageResponse>>
                {
                    code = 500,
                    message = "Internal server error occurred while retrieving favorites"
                };
            }
        }

        [HttpPost]
        public async Task<ApiResponse<object>> AddFavorite([FromBody] AddFavoriteRequest request)
        {
            try
            {
                await favoriteService.AddFavoriteAsync(request);

                return new ApiResponse<object>(
                    code: 201,
                    message : "Added to favorites successfully"
                );
            }
            catch (Exception ex)
            {
                return new ApiResponse<object>
                {
                    code = 500,
                    message = "Internal server error occurred while adding favorite"
                };
            }
        }

        [HttpDelete]
        public async Task<ApiResponse<object>> RemoveFavorite([FromBody] RemoveFavoriteRequest request)
        {
            try
            {
                await favoriteService.RemoveFavoriteAsync(request);

                return new ApiResponse<object>(
                    code: 200,
                    message : "Removed from favorites successfully"
                );
            }
            catch (Exception ex)
            {
                return new ApiResponse<object>
                {
                    code = 500,
                    message = "Internal server error occurred while removing favorite"
                };
            }
        }
    }
}
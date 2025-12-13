using AutoMapper;
using AutoMapper.QueryableExtensions;
using Cultural_Heritage_System.Dtos.Request;
using Cultural_Heritage_System.Dtos.Response;
using Cultural_Heritage_System.Dtos.Response.Heritage;
using Cultural_Heritage_System.Helpers;
using Cultural_Heritage_System.Middlewares;
using Cultural_Heritage_System.Models;
using Cultural_Heritage_System.Repositories;
using Cultural_Heritage_System.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;

namespace Cultural_Heritage_System.Services.Impl
{
    public class FavoriteService : IFavoriteService
    {
        private readonly IFavoriteRepository favoriteRepository;
        private readonly IHeritageRepository heritageRepository;
        private readonly IMapper mapper;
        private readonly ILogger<FavoriteService> logger;
        private readonly IHttpContextAccessor httpContextAccessor;

        public FavoriteService(
            IFavoriteRepository favoriteRepository,
            IHeritageRepository heritageRepository,
            IMapper mapper,
            ILogger<FavoriteService> logger,
            IHttpContextAccessor httpContextAccessor)
        {
            this.favoriteRepository = favoriteRepository;
            this.heritageRepository = heritageRepository;
            this.mapper = mapper;
            this.logger = logger;
            this.httpContextAccessor = httpContextAccessor;
        }

        public async Task<PageResponse<FavoriteHeritageResponse>> GetFavoritesAsync(int page, int pageSize, string? searchName)
        {
            try
            {
                var userId = GetCurrentUserId();
                if (userId == null)
                {
                    throw new AppException(ErrorCode.UNAUTHORIZED);
                }

                var query = favoriteRepository.GetFavoritesQueryByUserId(userId.Value);

                // Apply search filter if searchName is provided
                if (!string.IsNullOrEmpty(searchName))
                {
                    var searchTerm = searchName.Trim().ToLower();
                    var unsignedTerm = StringHelper.RemoveDiacritics(searchTerm);

                    query = query.Where(f =>
                        f.Heritage.Name.ToLower().Contains(searchTerm) ||
                        f.Heritage.NameUnsigned.Contains(unsignedTerm));
                }

                // Pagination
                //var pagedResponse = await query.ToPagedResponseAsync(page, pageSize);

                // Map to response DTOs
                var dtoQuery = query.ProjectTo<FavoriteHeritageResponse>(mapper.ConfigurationProvider);

                return  await dtoQuery.ToPagedResponseAsync(page, pageSize);
                
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error getting favorites for current user");
                throw;
            }
        }

        //public async Task<PageResponse<FavoriteHeritageResponse>> GetFavoritesByUserIdAsync(int userId, int page, int pageSize)
        //{
        //    try
        //    {
        //        var query = favoriteRepository.GetFavoritesQueryByUserId(userId);

        //        // Pagination
        //        var pagedResponse = await query.ToPagedResponseAsync(page, pageSize);

        //        // Map to response DTOs
        //        var favoriteResponses = mapper.Map<List<FavoriteHeritageResponse>>(pagedResponse.Items);

        //        return new PageResponse<FavoriteHeritageResponse>
        //        {
        //            CurrentPages = pagedResponse.CurrentPages,
        //            PageSizes = pagedResponse.PageSizes,
        //            TotalPages = pagedResponse.TotalPages,
        //            TotalElements = pagedResponse.TotalElements,
        //            Items = favoriteResponses
        //        };
        //    }
        //    catch (Exception ex)
        //    {
        //        logger.LogError(ex, "Error getting favorites for user {UserId}", userId);
        //        throw;
        //    }
        //}

        public async Task AddFavoriteAsync(AddFavoriteRequest request)
        {
            try
            {
                var userId = GetCurrentUserId();
                if (userId == null)
                {
                    throw new AppException(ErrorCode.UNAUTHORIZED);
                }

                // Check if heritage exists
                var heritage = await heritageRepository.GetHeritageByIdAsync(request.HeritageId);
                if (heritage == null)
                {
                    logger.LogError($"Heritage with ID {request.HeritageId} not found");
                    throw new AppException(ErrorCode.HERITAGE_NOT_FOUND);
                }

                // Check if favorite already exists
                var existingFavorite = await favoriteRepository.GetFavoriteByUserAndHeritageAsync(userId.Value, request.HeritageId);
                if (existingFavorite != null)
                {
                    logger.LogError($"Favorite already exists for user {userId} and heritage {request.HeritageId}");
                    throw new AppException(ErrorCode.FAVORITE_ALREADY_EXISTS);
                }

                // Create new favorite
                var favorite = new Favorite
                {
                    UserId = userId.Value,
                    HeritageId = request.HeritageId
                };

                await favoriteRepository.AddAsync(favorite);
                logger.LogInformation($"Added favorite for user {userId} and heritage {request.HeritageId}");
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error adding favorite for current user and heritage {HeritageId}", request.HeritageId);
                throw;
            }
        }

        public async Task RemoveFavoriteAsync(RemoveFavoriteRequest request)
        {
            try
            {
                var userId = GetCurrentUserId();
                if (userId == null)
                {
                    throw new AppException(ErrorCode.UNAUTHORIZED);
                }

                // Check if heritage exists
                var heritage = await heritageRepository.GetHeritageByIdAsync(request.HeritageId);
                if (heritage == null)
                {
                    logger.LogError($"Heritage with ID {request.HeritageId} not found");
                    throw new AppException(ErrorCode.HERITAGE_NOT_FOUND);
                }

                // Check if favorite exists
                var existingFavorite = await favoriteRepository.GetFavoriteByUserAndHeritageAsync(userId.Value, request.HeritageId);
                if (existingFavorite == null)
                {
                    logger.LogError($"Favorite not found for user {userId} and heritage {request.HeritageId}");
                    throw new AppException(ErrorCode.FAVORITE_NOT_FOUND);
                }

                // Remove favorite
                await favoriteRepository.DeleteAsync(existingFavorite);
                logger.LogInformation($"Removed favorite for user {userId} and heritage {request.HeritageId}");
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error removing favorite for current user and heritage {HeritageId}", request.HeritageId);
                throw;
            }
        }

        private int? GetCurrentUserId()
        {
            var accountIdClaim = httpContextAccessor.HttpContext?.User.FindFirst("userId")?.Value;
            if (string.IsNullOrEmpty(accountIdClaim))
            {
                return null;
            }

            if (int.TryParse(accountIdClaim, out int userId))
            {
                return userId;
            }

            return null;
        }
    }
}
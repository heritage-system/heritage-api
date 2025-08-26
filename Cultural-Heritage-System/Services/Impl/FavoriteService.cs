using AutoMapper;
using Cultural_Heritage_System.Dtos.Request;
using Cultural_Heritage_System.Dtos.Response;
using Cultural_Heritage_System.Helpers;
using Cultural_Heritage_System.Middlewares;
using Cultural_Heritage_System.Models;
using Cultural_Heritage_System.Repositories;
using Cultural_Heritage_System.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;

namespace Cultural_Heritage_System.Services.Impl
{
    public class FavoriteService : IFavoriteService
    {
        private readonly FavoriteRepository favoriteRepository;
        private readonly HeritageRepository heritageRepository;
        private readonly IMapper mapper;
        private readonly ILogger<FavoriteService> logger;

        public FavoriteService(
            FavoriteRepository favoriteRepository,
            HeritageRepository heritageRepository,
            IMapper mapper,
            ILogger<FavoriteService> logger)
        {
            this.favoriteRepository = favoriteRepository;
            this.heritageRepository = heritageRepository;
            this.mapper = mapper;
            this.logger = logger;
        }

        public async Task<PageResponse<FavoriteHeritageResponse>> GetFavoritesByUserIdAsync(int userId, int page, int pageSize)
        {
            try
            {
                var query = favoriteRepository.GetFavoritesQueryByUserId(userId);

                // Pagination
                var pagedResponse = await query.ToPagedResponseAsync(page, pageSize);

                // Map to response DTOs
                var favoriteResponses = mapper.Map<List<FavoriteHeritageResponse>>(pagedResponse.Items);

                return new PageResponse<FavoriteHeritageResponse>
                {
                    CurrentPages = pagedResponse.CurrentPages,
                    PageSizes = pagedResponse.PageSizes,
                    TotalPages = pagedResponse.TotalPages,
                    TotalElements = pagedResponse.TotalElements,
                    Items = favoriteResponses
                };
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error getting favorites for user {UserId}", userId);
                throw;
            }
        }

        public async Task AddFavoriteAsync(int userId, AddFavoriteRequest request)
        {
            try
            {
                // Check if heritage exists
                var heritage = await heritageRepository.GetHeritageByIdAsync(request.HeritageId);
                if (heritage == null)
                {
                    logger.LogError($"Heritage with ID {request.HeritageId} not found");
                    throw new AppException(ErrorCode.HERITAGE_NOT_FOUND);
                }

                // Check if favorite already exists
                var existingFavorite = await favoriteRepository.GetFavoriteByUserAndHeritageAsync(userId, request.HeritageId);
                if (existingFavorite != null)
                {
                    logger.LogError($"Favorite already exists for user {userId} and heritage {request.HeritageId}");
                    throw new AppException(ErrorCode.FAVORITE_ALREADY_EXISTS);
                }

                // Create new favorite
                var favorite = new Favorite
                {
                    UserId = userId,
                    HeritageId = request.HeritageId
                };

                await favoriteRepository.AddAsync(favorite);
                logger.LogInformation($"Added favorite for user {userId} and heritage {request.HeritageId}");
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error adding favorite for user {UserId} and heritage {HeritageId}", userId, request.HeritageId);
                throw;
            }
        }

        public async Task RemoveFavoriteAsync(int userId, RemoveFavoriteRequest request)
        {
            try
            {
                // Check if heritage exists
                var heritage = await heritageRepository.GetHeritageByIdAsync(request.HeritageId);
                if (heritage == null)
                {
                    logger.LogError($"Heritage with ID {request.HeritageId} not found");
                    throw new AppException(ErrorCode.HERITAGE_NOT_FOUND);
                }

                // Check if favorite exists
                var existingFavorite = await favoriteRepository.GetFavoriteByUserAndHeritageAsync(userId, request.HeritageId);
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
                logger.LogError(ex, "Error removing favorite for user {UserId} and heritage {HeritageId}", userId, request.HeritageId);
                throw;
            }
        }
    }
}
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Azure.Core;
using Cultural_Heritage_System.Common;
using Cultural_Heritage_System.Dtos.Models;
using Cultural_Heritage_System.Dtos.Request.Heritage;
using Cultural_Heritage_System.Dtos.Request.UserPoint;
using Cultural_Heritage_System.Dtos.Response;
using Cultural_Heritage_System.Dtos.Response.Heritage;
using Cultural_Heritage_System.Dtos.Response.UserPoint;
using Cultural_Heritage_System.Helpers;
using Cultural_Heritage_System.Middlewares;
using Cultural_Heritage_System.Models;
using Cultural_Heritage_System.Repositories;
using Cultural_Heritage_System.Repositories.Impl;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace Cultural_Heritage_System.Services.Impl
{
    public class UserPointService : IUserPointService
    {
        private readonly IHttpContextAccessor httpContextAccessor;
        private readonly IUserPointRepository userPointRepository;
        private readonly IPointHistoryRepository pointHistoryRepository;
        private readonly IUserRepository userRepository;
        private readonly ISubscriptionRepository subscriptionRepository;
        private readonly IMapper mapper;

        private readonly ILogger<UserPointService> logger;

        public UserPointService(IUserPointRepository userPointRepository, IUserRepository userRepository, ILogger<UserPointService> logger, IMailService mailService,
            IMapper mapper, IHttpContextAccessor httpContextAccessor, IPointHistoryRepository pointHistoryRepository,
            ISubscriptionRepository subscriptionRepository)
        {
            this.userPointRepository = userPointRepository;
            this.userRepository = userRepository;
            this.logger = logger;
            this.mapper = mapper;
            this.httpContextAccessor = httpContextAccessor;
            this.subscriptionRepository = subscriptionRepository;
            this.pointHistoryRepository = pointHistoryRepository;

        }

        public async Task<UserPointResponse> GetUserPointByUserId()
        {
            var accountIdClaim = httpContextAccessor.HttpContext?.User.FindFirst("userId")?.Value;
            if (accountIdClaim == null)
            {
                throw new AppException(ErrorCode.UNAUTHORIZED);
            }

            int userId = int.Parse(accountIdClaim);
            var userPoint = await userPointRepository.GetUserPointByUserId(userId);
            if (userPoint == null)
            {
                var newUserPoint = new UserPoint
                {
                    UserId = userId,
                    TotalPoints = 0,
                };             
                await userPointRepository.AddAsync(newUserPoint);
                return new UserPointResponse
                {
                    UserId = userId,
                    TotalPoints = 0,
                    UnlockTokens = 0
                };
            }
            else
            {
                return mapper.Map<UserPointResponse>(userPoint);
            }
        }

        public async Task<bool> UpdateUserPoint(UserPointUpdateRequest request)
        {
            try
            {
                var user = await userRepository.FindUserById(request.UserId);
                if (user == null) {
                    throw new AppException(ErrorCode.UNAUTHORIZED);
                }

                var oldTotalPoints = 0;
                var newTotalPoints = 0;
                var userPoint = await userPointRepository.GetUserPointByUserId(request.UserId);
                if (userPoint == null)
                {
                    var newUserPoint = new UserPoint
                    {
                        UserId = request.UserId,
                        TotalPoints = request.ChangeAmount,
                    };
                    newTotalPoints = newUserPoint.TotalPoints;
                    await userPointRepository.AddAsync(newUserPoint);
                }
                else
                {
                    oldTotalPoints = userPoint.TotalPoints;
                    // --- Update basic fields ---
                    userPoint.TotalPoints += request.ChangeAmount;

                    userPoint.UpdatedAt = DateTime.Now;

                    await userPointRepository.UpdateAsync(userPoint);
                    newTotalPoints = userPoint.TotalPoints;
                }

                var pointHistory = new PointHistory
                {
                    ChangeAmount = request.ChangeAmount,
                    UserId = request.UserId,
                    OldValue = oldTotalPoints,
                    NewValue = newTotalPoints,
                    Reason = request.Reason,
                    ReferenceId = request.ReferenceId,
                };

                await pointHistoryRepository.AddAsync(pointHistory);

                return true;
            }
            catch
            {
                return false;
            }
        }
    }

      
}

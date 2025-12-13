using AutoMapper;
using AutoMapper.QueryableExtensions;
using Azure.Core;
using Cultural_Heritage_System.Common;
using Cultural_Heritage_System.Dtos.Models;
using Cultural_Heritage_System.Dtos.Request.Heritage;
using Cultural_Heritage_System.Dtos.Response;
using Cultural_Heritage_System.Dtos.Response.GameMatchHistory;
using Cultural_Heritage_System.Dtos.Response.Heritage;
using Cultural_Heritage_System.Helpers;
using Cultural_Heritage_System.Middlewares;
using Cultural_Heritage_System.Models;
using Cultural_Heritage_System.Repositories;
using Cultural_Heritage_System.Repositories.Impl;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;

namespace Cultural_Heritage_System.Services.Impl
{
    public class GameMatchHistoryService : IGameMatchHistoryService
    {
        private readonly IHttpContextAccessor httpContextAccessor;
        private readonly IGameMatchHistoryRepository gameMatchHistoryRepository;
        private readonly IPointHistoryRepository pointHistoryRepository;
        private readonly IUserRepository userRepository;
        private readonly ISubscriptionRepository subscriptionRepository;
        private readonly IMapper mapper;

        private readonly ILogger<GameMatchHistoryService> logger;

        public GameMatchHistoryService(IGameMatchHistoryRepository gameMatchHistoryRepository, IUserRepository userRepository, ILogger<GameMatchHistoryService> logger, IMailService mailService,
            IMapper mapper, IHttpContextAccessor httpContextAccessor, IPointHistoryRepository pointHistoryRepository,
            ISubscriptionRepository subscriptionRepository)
        {
            this.gameMatchHistoryRepository = gameMatchHistoryRepository;
            this.userRepository = userRepository;
            this.logger = logger;
            this.mapper = mapper;
            this.httpContextAccessor = httpContextAccessor;
            this.subscriptionRepository = subscriptionRepository;
            this.pointHistoryRepository = pointHistoryRepository;

        }

        public async Task<bool> CreateGameMatchHistory(GameMatchHistory request)
        {
            try
            {
                //var gameMatch = new GameMatchHistory
                //{
                //    MatchId = gameSession.Id,
                //    MatchType = gameSession.RoomType,
                //    QuestionCount = gameSession.Questions.Count,
                //    Player1Id = gameSession.Players[0].UserId,
                //    Player1Name = gameSession.Players[0].Username,
                //    Player1Avatar = gameSession.Players[0].AvatarUrl,
                //    Player1Score = gameSession.Players[0].Score,
                //    Player1IP = gameSession.Players[0].IpAddress,
                //    Player2Id = gameSession.Players[1].UserId == -1 ? null : gameSession.Players[1].UserId,
                //    Player2Name = gameSession.Players[1].Username,
                //    Player2Avatar = gameSession.Players[1].AvatarUrl,
                //    Player2Score = gameSession.Players[1].Score,
                //    Player2IP = gameSession.Players[1].IpAddress,
                //    WinnerPlayer = winner,
                //    PlusPoint = plusPoint
                //};

                await gameMatchHistoryRepository.AddAsync(request);              
                return true;
            }
            catch
            {
                return false;
            }
        }

        public async Task<List<UserMatchHistoryResponse>> GetUserGameMatchHistory()
        {
            var accountIdClaim = httpContextAccessor.HttpContext?.User.FindFirst("userId")?.Value;
            if (string.IsNullOrEmpty(accountIdClaim))
            {
                throw new AppException(ErrorCode.UNAUTHORIZED);
            }
            int userId = int.Parse(accountIdClaim);
            var query = await gameMatchHistoryRepository
                .GetQueryable()
                .Where(g => g.Player1Id == userId || g.Player2Id == userId)
                .OrderByDescending(g => g.CreatedAt) 
                .Take(20)
                .ToListAsync();           

            var response =  mapper.Map<List<UserMatchHistoryResponse>>(query);

            foreach (var item in response)
            {
                item.UserNumber = item.Player1Id == userId ? 1 : 2;
            }

            return response;

        }

        public async Task<bool> IsSpamMatch(int userId, string userIP, int opponentId, string opponentIP)
        {
            var cutoff = DateTime.UtcNow.AddDays(-2);

            // Lấy match trong 48 giờ gần nhất của cả hai
            var recentMatches = await gameMatchHistoryRepository.GetQueryable()
                .Where(m => m.CreatedAt >= cutoff)
                .ToListAsync();

            // 1. Hai tài khoản này đã chơi với nhau rồi → spam
            bool playedTogether = recentMatches.Any(m =>
                (m.Player1Id == userId && m.Player2Id == opponentId) ||
                (m.Player1Id == opponentId && m.Player2Id == userId)
            );

            if (playedTogether) return true;

            //// 2. Trùng IP trực tiếp → spam
            //if (!string.IsNullOrEmpty(userIP) && !string.IsNullOrEmpty(opponentIP))
            //{
            //    if (userIP == opponentIP)
            //        return true;
            //}

            //// 3. userId từng đánh với AI có IP trùng opponentIP → spam (giả danh)
            //bool userPlayedWithSameIP = recentMatches.Any(m =>
            //    (m.Player1Id == userId && m.Player2IP == opponentIP) ||
            //    (m.Player2Id == userId && m.Player1IP == opponentIP)
            //);

            //if (userPlayedWithSameIP) return true;

            //// 4. opponent từng đánh với tài khoản IP giống userIP
            //bool opponentPlayedWithSameIP = recentMatches.Any(m =>
            //    (m.Player1Id == opponentId && m.Player2IP == userIP) ||
            //    (m.Player2Id == opponentId && m.Player1IP == userIP)
            //);

            //if (opponentPlayedWithSameIP) return true;

            return false; // OK, không phải spam
        }

    }

}

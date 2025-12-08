using Cultural_Heritage_System.Common;
using Cultural_Heritage_System.Dtos.Models;
using Cultural_Heritage_System.Dtos.Request.UserPoint;
using Cultural_Heritage_System.Dtos.Response.QuizQuestion;
using Cultural_Heritage_System.Middlewares;
using Cultural_Heritage_System.Models;
using Cultural_Heritage_System.Repositories;
using Cultural_Heritage_System.Repositories.Impl;
using Cultural_Heritage_System.Services;
using Cultural_Heritage_System.Services.Impl;
using Microsoft.AspNetCore.SignalR;

namespace Cultural_Heritage_System.GameHubs
{
    public class GameHub : Hub
    {
        private readonly IHubContext<GameHub> _hubContext;
        private static readonly Dictionary<string, GameSession> Sessions = new();
        private static readonly List<WaitingPlayer> WaitingPlayers = new();
        private static readonly Dictionary<string, List<QuizQuestionResponse>> PreGeneratedQuestions = new();
        private readonly IQuizService _quizService;
        private readonly IUserPointService _userPointService;
        private readonly IGameMatchHistoryService _gameMatchHistoryService;
        private readonly IUserService _userService;
        private readonly IServiceScopeFactory _scopeFactory;

        private const int QUESTION_TIME = 10; // giây
        private const int READING_TIME = 3;
        private const int QUESTION_QUANTITY = 5;
        private const int MIN_TIME_BOT_ANSWER = 1;
        private const int MAX_TIME_BOT_ANSWER = 3;
        // === DỮ LIỆU MẪU ===

        public GameHub(IHubContext<GameHub> hubContext, IQuizService quizService,IUserPointService userPointService, IUserService userService, IServiceScopeFactory scopeFactory, IGameMatchHistoryService gameMatchHistoryService)
        {
            _hubContext = hubContext;
            _quizService = quizService;
            _userPointService = userPointService;
            _userService = userService;
            _scopeFactory = scopeFactory;
            _gameMatchHistoryService = gameMatchHistoryService;
        }

        // === GHÉP TRẬN ===
        public async Task FindMatch(string username, string avatarUrl)
        {
            var accountIdClaim = Context.User?.FindFirst("userId")?.Value;
            if (accountIdClaim == null)
                throw new AppException(ErrorCode.UNAUTHORIZED);

            var userId = int.Parse(accountIdClaim);
            var connectionId = Context.ConnectionId;

            // Xóa chờ cũ (nếu có)
            var existing = WaitingPlayers.FirstOrDefault(p => p.ConnectionId == connectionId);
            if (existing != null)
            {
                existing.CancelToken.Cancel();
                WaitingPlayers.Remove(existing);
            }

            // Kiểm tra session cũ
            var inSession = Sessions.Values.FirstOrDefault(s => s.Players.Any(p => p.ConnectionId == connectionId));
            if (inSession != null)
            {
                Sessions.Remove(Sessions.First(x => x.Value == inSession).Key);
            }

            var ip = GetClientIp();

            // Tạo player
            var player = new Player
            {
                Id = Guid.NewGuid(),
                UserId = userId,
                Username = username,
                AvatarUrl = avatarUrl,
                ConnectionId = connectionId,
                IpAddress = ip
            };

            await Clients.Caller.SendAsync("WaitingForOpponent");

            // Nếu có người chờ → ghép ngay
            var waiting = WaitingPlayers.FirstOrDefault(p => p.ConnectionId != connectionId);
            if (waiting != null)
            {
                // --- AntiSpam: Check 2 thằng này có được ghép hay không ---
                bool isSpam = await _gameMatchHistoryService.IsSpamMatch(
                    userId: player.UserId,
                    userIP: player.IpAddress,
                    opponentId: waiting.Player.UserId,
                    opponentIP: waiting.Player.IpAddress
                );

                if (!isSpam)
                {
                    // OK, GHÉP BÌNH THƯỜNG
                    waiting.CancelToken.Cancel();
                    WaitingPlayers.Remove(waiting);

                    await CreateMatch(waiting.Player, player, waiting.ConnectionId, connectionId);
                    return;
                }
                else
                {                
                    Console.WriteLine("⛔ AntiSpam: phát hiện spam → không ghép cặp này.");
                }
            }

            var preQuestions = await _quizService.GenerateQuestionSet(QUESTION_QUANTITY);
            PreGeneratedQuestions[connectionId] = preQuestions;


            // ➕ Không ai chờ → đưa vào hàng chờ + bật timer 30s
            var cancelToken = new CancellationTokenSource();
            var waitingObj = new WaitingPlayer
            {
                ConnectionId = connectionId,
                Player = player,
                AddedAt = DateTime.UtcNow,
                CancelToken = cancelToken
            };

            WaitingPlayers.Add(waitingObj);

            _ = Task.Run(async () =>
            {
                try
                {
                    var rnd = new Random();
                    int delayMs = rnd.Next(2000, 5000); // 5000ms = 5s, 20000ms = 20s
                    await Task.Delay(delayMs, cancelToken.Token);

                    // Nếu sau 30s mà vẫn còn trong hàng chờ → đấu bot
                    if (WaitingPlayers.Contains(waitingObj))
                    {
                        WaitingPlayers.Remove(waitingObj);
                        Console.WriteLine($"🤖 {username} chờ quá {delayMs / 1000}s → đấu với BOT");

                        await CreateBotMatch(player, connectionId);
                    }
                }
                catch (TaskCanceledException)
                {
                    // Bị ghép rồi
                }
            });

           
          
        }

        private string? GetClientIp()
        {
            var http = Context.GetHttpContext();
            if (http == null) return null;

            // Nếu có proxy
            if (http.Request.Headers.TryGetValue("X-Forwarded-For", out var forwarded))
            {
                var raw = forwarded.ToString().Split(',').FirstOrDefault();
                if (!string.IsNullOrWhiteSpace(raw)) return raw.Trim();
            }

            return http.Connection.RemoteIpAddress?.ToString();
        }


        private async Task CreateMatch(Player p1, Player p2, string c1, string c2)
        {
            var roomId = Guid.NewGuid().ToString();

            var questions = PreGeneratedQuestions.ContainsKey(c1)
                ? PreGeneratedQuestions[c1]
                : await _quizService.GenerateQuestionSet(QUESTION_QUANTITY);

            var session = new GameSession
            {
                Id = Guid.Parse(roomId),
                Name = $"Match {roomId}",
                Players = new List<Player> { p1, p2 },
                Questions = questions,
                CurrentQuestionIndex = 0
            };

            Sessions[roomId] = session;
            PreGeneratedQuestions.Remove(c1);
            PreGeneratedQuestions.Remove(c2);

            await Groups.AddToGroupAsync(c1, roomId);
            await Groups.AddToGroupAsync(c2, roomId);

            await Clients.Client(c1).SendAsync("MatchFound", roomId, session.Players);
            await Clients.Client(c2).SendAsync("MatchFound", roomId, session.Players);
        }

        private async Task CreateBotMatch(Player player, string connectionId)
        {

            using var scope = _scopeFactory.CreateScope();
            var userService = scope.ServiceProvider.GetRequiredService<IUserService>();

            var botUser = await userService.GetRandomUserExcept(player.UserId);

            var bot = new Player
            {
                Id = Guid.NewGuid(),
                UserId = -1,
                Username = botUser.UserName,
                AvatarUrl = botUser.Profile.AvatarUrl,
                ConnectionId = "BOT"
            };

            var roomId = Guid.NewGuid().ToString();

            var questions = PreGeneratedQuestions.ContainsKey(connectionId)
                ? PreGeneratedQuestions[connectionId]
                : await _quizService.GenerateQuestionSet(QUESTION_QUANTITY);

            var session = new GameSession
            {
                Id = Guid.Parse(roomId),
                Name = $"BotMatch {roomId}",
                Players = new List<Player> { player, bot },
                Questions = questions,
                CurrentQuestionIndex = 0,
                RoomType = RoomType.BOT
            };

            Sessions[roomId] = session;
            PreGeneratedQuestions.Remove(connectionId);

            await _hubContext.Groups.AddToGroupAsync(connectionId, roomId);
            await _hubContext.Clients.Client(connectionId).SendAsync("MatchFound", roomId, session.Players);
        }
      



        // === GỬI CÂU HỎI ===
        private async Task SendQuestion(string roomId)
        {
            if (!Sessions.TryGetValue(roomId, out var session)) return;
            if (session.CurrentQuestionIndex >= session.Questions.Count) return;
            var q = session.CurrentQuestion;
            var startTime = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();

            session.TimerCts?.Cancel();
            session.TimerCts = new CancellationTokenSource();


            session.Answers.Clear();
            session.IsRevealing = false;
            session.QuestionStartTime = startTime;

            await _hubContext.Clients.Group(roomId).SendAsync("StartQuestion", new
            {
                id = q.Id,
                question = q.Question,
                options = q.ToOptionsArray(),
                difficulty = q.QuizLevel,
                readingDuration = READING_TIME,
                answerDuration = QUESTION_TIME,
                startTimeUtcMs = startTime
            });

            foreach (var p in session.Players)
                p.LastAnswerTime = 0;

            // Bắt đầu đếm giờ toàn cục (background)
            _ = RunQuestionTimer(roomId, session.TimerCts.Token);
            var existingBot = session.Players.FirstOrDefault(p => p.ConnectionId == "BOT");
            if (existingBot != null)
            {
                _ = Task.Run(async () => await SimulateBotAnswer(session, existingBot));
            }
            return;
        }

        // === TIMER NỀN ===
        private async Task RunQuestionTimer(string roomId, CancellationToken token)
        {
            try
            {
                await Task.Delay(TimeSpan.FromSeconds(READING_TIME + QUESTION_TIME), token);

                if (token.IsCancellationRequested) return;

                if (!Sessions.TryGetValue(roomId, out var s)) return;

                if (!s.IsRevealing && s.Answers.Count < s.Players.Count)
                {
                    Console.WriteLine($"⌛ [AUTO-REVEAL] Room {roomId}: timeout reached.");
                    await RevealAndNextQuestion(roomId, useHubContext: true);
                }
            }
            catch (TaskCanceledException)
            {
                // ⏹ Timer bị hủy – không làm gì cả
                Console.WriteLine($"🛑 Timer cancelled for room {roomId}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ [AUTO-REVEAL] Error in room {roomId}: {ex.Message}");
            }
        }

        


        // === NGƯỜI CHƠI GỬI ĐÁP ÁN ===
        public async Task SubmitAnswer(string roomId, string playerId, int questionId, int answerIndex, double elapsedTime)
        {
            if (!Sessions.TryGetValue(roomId, out var session)) return;
            var player = session.Players.FirstOrDefault(p => p.Id.ToString() == playerId);
            if (player == null) return;

            player.LastAnswerTime = Math.Round(elapsedTime, 2);
            session.Answers[playerId] = (answerIndex, player.LastAnswerTime);

            await _hubContext.Clients.Group(roomId).SendAsync("PlayerAnswered", new
            {
                playerId,
                elapsedTime = player.LastAnswerTime
            });

            // Khi tất cả đã trả lời
            if (session.Answers.Count >= session.Players.Count && !session.IsRevealing)
            {
                await RevealAndNextQuestion(roomId, useHubContext: true);
            }
        }

        // === CHẤM ĐIỂM + GỬI TIẾP CÂU HỎI ===
        private async Task RevealAndNextQuestion(string roomId, bool useHubContext = false)
        {
            if (!Sessions.TryGetValue(roomId, out var session)) return;
            if (session.IsRevealing) return;
            session.IsRevealing = true;

            var q = session.CurrentQuestion;
            int correctIndex = LetterToIndex(q.CorrectOption);

            // 🎯 Xác định điểm tối đa theo cấp độ
            int maxPoints = q.QuizLevel switch
            {
                "EASY" => 100,
                "MEDIUM" => 200,
                "HARD" => 300,
                _ => 100
            };

            //const int QUESTION_TIME = 13; // giây

            // 🧮 Tính điểm từng người
            foreach (var (playerId, (ansIndex, elapsed)) in session.Answers)
            {
                var player = session.Players.FirstOrDefault(p => p.Id.ToString() == playerId);
                if (player == null) continue;

                bool isCorrect = ansIndex == correctIndex;
                if (isCorrect)
                {
                    // công thức: càng nhanh càng nhiều điểm
                    double effectiveElapsed = Math.Max(0, elapsed - READING_TIME);
                    double ratio = Math.Max(0, 1 - effectiveElapsed / QUESTION_TIME);
                    int gained = (int)Math.Round(maxPoints * ratio);
                    player.Score += gained;
                }
            }
            await Task.Delay(500);

            var payload = new
            {
                correctIndex,
                scores = session.Players.Select(p => new
                {
                    p.Id,
                    p.Username,
                    p.Score,
                    p.LastAnswerTime
                })
            };

            if (useHubContext)
                await _hubContext.Clients.Group(roomId).SendAsync("RevealAnswer", payload);
            else
                await Clients.Group(roomId).SendAsync("RevealAnswer", payload);

            // chuẩn bị câu tiếp theo
            session.Answers.Clear();
            session.CurrentQuestionIndex++;

            if (session.CurrentQuestionIndex < session.Questions.Count)
            {
                session.CurrentQuestion = session.Questions[session.CurrentQuestionIndex];
                await Task.Delay(1000);
                await SendQuestion(roomId);
            }
            else
            {
                await Task.Delay(1000);               
                await AwardPointsAfterMatch(roomId, session);        
                
                if (useHubContext)
                   
                    await _hubContext.Clients.Group(roomId).SendAsync("GameFinished", session.Players);
                else
                    await Clients.Group(roomId).SendAsync("GameFinished", session.Players);

                foreach (var p in session.Players)
                {
                    await Groups.RemoveFromGroupAsync(p.ConnectionId, roomId);
                }

            }
        }


        private async Task AwardPointsAfterMatch(string roomId, GameSession gameSession)
        {
            var players = gameSession.Players;
            if (players.Count != 2) return;

            var p1 = players[0];
            var p2 = players[1];

            Winner winnerResult;
            Player? winner = null;
            Player? loser = null;

            // -------------------------
            // XÁC ĐỊNH KẾT QUẢ TRẬN
            // -------------------------
            if (p1.Score == p2.Score)
            {
                winnerResult = Winner.DRAW;
            }
            else if (p1.Score > p2.Score)
            {
                winnerResult = Winner.PLAYER_1;
                winner = p1;
                loser = p2;
            }
            else
            {
                winnerResult = Winner.PLAYER_2;
                winner = p2;
                loser = p1;
            }

            // -------------------------
            // TÍNH ĐIỂM THƯỞNG
            // -------------------------
            int totalGain = 0;

            if (winnerResult != Winner.DRAW)
            {
                int diff = Math.Abs(p1.Score - p2.Score);

                int baseGain = 20;
                int bonusGain = diff / 20;
                totalGain = baseGain + bonusGain;

                if (totalGain > 60)
                    totalGain = 60;
            }

            // -------------------------
            // LƯU MATCH HISTORY
            // -------------------------
            using var scope = _scopeFactory.CreateScope();
            var gameMatchHistoryService =
                scope.ServiceProvider.GetRequiredService<IGameMatchHistoryService>();

            var gameMatch = new GameMatchHistory
            {
                MatchId = gameSession.Id,
                MatchType = gameSession.RoomType,
                QuestionCount = gameSession.Questions.Count,
                Player1Id = gameSession.Players[0].UserId,
                Player1Name = gameSession.Players[0].Username,
                Player1Avatar = gameSession.Players[0].AvatarUrl,
                Player1Score = gameSession.Players[0].Score,
                Player1IP = gameSession.Players[0].IpAddress,
                Player2Id = gameSession.Players[1].UserId == -1 ? null : gameSession.Players[1].UserId,
                Player2Name = gameSession.Players[1].Username,
                Player2Avatar = gameSession.Players[1].AvatarUrl,
                Player2Score = gameSession.Players[1].Score,
                Player2IP = gameSession.Players[1].IpAddress,
                WinnerPlayer = winnerResult,
                PlusPoint = totalGain
            };
            await gameMatchHistoryService.CreateGameMatchHistory(
                gameMatch
            );

            // -------------------------
            // BOT thì KHÔNG TÍNH ĐIỂM
            // -------------------------
            if (winnerResult != Winner.DRAW && winner?.ConnectionId == "BOT")
                return;

            // -------------------------
            // UPDATE POINT RANKING
            // -------------------------
            if (winnerResult != Winner.DRAW &&
                (gameSession.RoomType == RoomType.RANDOM ||
                 gameSession.RoomType == RoomType.BOT))
            {
                winner!.BonusPoint = totalGain;

                var userPoint = new UserPointUpdateRequest
                {
                    UserId = winner.UserId,
                    ChangeAmount = totalGain,
                    Reason = PointHistoriesReason.PVP_WIN
                };

                await _userPointService.UpdateUserPoint(userPoint);
            }
        }




        private static int LetterToIndex(string? letter) => letter?.Trim().ToUpper() switch
        {
            "A" => 0,
            "B" => 1,
            "C" => 2,
            "D" => 3,
            _ => -1
        };

        public async Task RequestCurrentQuestion(string roomId)
        {
            if (!Sessions.TryGetValue(roomId, out var session)) return;

            // ⚡ Nếu chưa có câu hỏi nào (trận mới bắt đầu)
            if (session.CurrentQuestion == null)
            {
                session.CurrentQuestionIndex = 0;
                session.CurrentQuestion = session.Questions[0];
                await SendQuestion(roomId); // broadcast + khởi tạo timer
                var existingBot = session.Players.FirstOrDefault(p => p.ConnectionId == "BOT");
                if (existingBot != null)
                {
                    _ = Task.Run(async () => await SimulateBotAnswer(session, existingBot));
                }
                return;
            }

            // ⚡ Nếu đang trong trận => chỉ gửi snapshot cho caller
            await Clients.Caller.SendAsync("StartQuestion", new
            {
                id = session.CurrentQuestion.Id,
                question = session.CurrentQuestion.Question,
                options = session.CurrentQuestion.ToOptionsArray(),
                difficulty = session.CurrentQuestion.QuizLevel.ToString(),
                startTimeUtcMs = session.QuestionStartTime,
                readingDuration = READING_TIME,
                answerDuration = QUESTION_TIME,

            });

           
        }


        // === NGẮT KẾT NỐI ===
        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            var conn = Context.ConnectionId;

            var waiting = WaitingPlayers.FirstOrDefault(p => p.ConnectionId == conn);
            if (waiting != null)
            {
                WaitingPlayers.Remove(waiting);
                Console.WriteLine($"⚠️ Player {waiting.Player.Username} rời hàng đợi");
            }

            var session = Sessions.Values.FirstOrDefault(s => s.Players.Any(p => p.ConnectionId == conn));
            if (session != null)
            {
                var otherConn = session.Players.FirstOrDefault(p => p.ConnectionId != conn)?.ConnectionId;
                if (otherConn != null)
                    await _hubContext.Clients.Client(otherConn).SendAsync("OpponentDisconnected");

                Sessions.Remove(Sessions.First(x => x.Value == session).Key);
            }

            await base.OnDisconnectedAsync(exception);
        }

        public async Task CancelFindMatch()
        {
            var connId = Context.ConnectionId;
            var waiting = WaitingPlayers.FirstOrDefault(p => p.ConnectionId == connId);

            if (waiting != null)
            {
                WaitingPlayers.Remove(waiting);
                Console.WriteLine($"🧹 {waiting.Player.Username} hủy tìm trận và rời hàng chờ");
            }

            await Clients.Caller.SendAsync("MatchCanceled");
        }

        // === TẠO PHÒNG CHƠI RIÊNG ===
        public async Task<string> CreateRoom(string username, string avatarUrl)
        {
            var connectionId = Context.ConnectionId;

            var accountIdClaim = Context.User?.FindFirst("userId")?.Value;
            if (accountIdClaim == null)
            {
                throw new AppException(ErrorCode.UNAUTHORIZED);
            }
            var userId = int.Parse(accountIdClaim);

            // 🧹 Xóa session cũ (nếu có)
            var existingSession = Sessions.Values.FirstOrDefault(s => s.Players.Any(p => p.ConnectionId == connectionId));
            if (existingSession != null)
            {
                Sessions.Remove(Sessions.First(x => x.Value == existingSession).Key);
            }

            var ip = GetClientIp();
            // 🧩 Tạo phòng mới
            var roomId = Guid.NewGuid().ToString("N").Substring(0, 6).ToUpper(); // ví dụ: "AB12CD"
            var player = new Player
            {
                Id = Guid.NewGuid(),
                UserId = userId,
                Username = username,
                AvatarUrl = avatarUrl,
                ConnectionId = connectionId,
                IpAddress = ip
            };

            var session = new GameSession
            {
                Id = Guid.NewGuid(),
                Name = $"Private Room {roomId}",
                Players = new List<Player> { player },
                Questions = await _quizService.GenerateQuestionSet(QUESTION_QUANTITY),
                CurrentQuestionIndex = 0,
                RoomType = RoomType.PLAY_WITH_FRIEND
            };
            Sessions[roomId] = session;

            await Groups.AddToGroupAsync(connectionId, roomId);
            await Clients.Caller.SendAsync("RoomCreated", roomId);

            Console.WriteLine($"🏠 {username} created private room {roomId}");
            return roomId;
        }

        // === THAM GIA PHÒNG ===
        public async Task JoinRoom(string roomCode, string username, string avatarUrl)
        {
            var connectionId = Context.ConnectionId;

            if (!Sessions.TryGetValue(roomCode, out var session))
            {
                await Clients.Caller.SendAsync("RoomNotFound", roomCode);
                Console.WriteLine($"❌ Join failed: {roomCode} not found.");
                return;
            }

            if (session.Players.Count >= 2)
            {
                await Clients.Caller.SendAsync("RoomFull", roomCode);
                Console.WriteLine($"🚫 Room {roomCode} full.");
                return;
            }

            var accountIdClaim = Context.User?.FindFirst("userId")?.Value;
            if (accountIdClaim == null)
            {
                throw new AppException(ErrorCode.UNAUTHORIZED);
            }
            var userId = int.Parse(accountIdClaim);

            var ip = GetClientIp();
            var player = new Player
            {
                Id = Guid.NewGuid(),
                UserId = userId,
                Username = username,
                AvatarUrl = avatarUrl,
                ConnectionId = connectionId,
                IpAddress = ip
            };

            session.Players.Add(player);
            await Groups.AddToGroupAsync(connectionId, roomCode);

            Console.WriteLine($"👫 {username} joined private room {roomCode}");

            // Gửi thông báo cho cả 2
            await _hubContext.Clients.Group(roomCode).SendAsync("RoomJoined", roomCode, session.Players);

            // ✅ Nếu đủ 2 người, bắt đầu
            if (session.Players.Count == 2)
            {
                //session.CurrentQuestionIndex = 0;
                //session.CurrentQuestion = session.Questions[0];

                await Task.Delay(1000);
                //await SendQuestion(roomCode);
            }
        }

        public async Task CancelMatch()
        {
            var connId = Context.ConnectionId;
            var waiting = WaitingPlayers.FirstOrDefault(p => p.ConnectionId == connId);

            if (waiting != null)
            {
                WaitingPlayers.Remove(waiting);
                Console.WriteLine($"🛑 {waiting.Player.Username} hủy tìm trận và rời hàng chờ.");
            }
            else
            {
                Console.WriteLine($"⚠️ Người chơi {connId} không nằm trong hàng chờ, không cần hủy.");
            }

            await Clients.Caller.SendAsync("MatchCanceled");
        }



        private async Task SimulateBotAnswer(GameSession session, Player bot)
        {
            var rnd = new Random();
            int min = READING_TIME * 1000 + MIN_TIME_BOT_ANSWER * 1000;
            int max = READING_TIME * 1000 + MAX_TIME_BOT_ANSWER * 1000;
            int delayMs = rnd.Next(min, max); 
            await Task.Delay(delayMs);

            // Chọn kiểu trả lời
            int chance = rnd.Next(1, 101); // 1 - 100
            int answerIndex = -1; // -1 = không trả lời

            if (chance <= 98)
            {
                // 90% → trả lời đúng
                answerIndex = LetterToIndex(session.CurrentQuestion.CorrectOption);
            }
            else if (chance <= 1)
            {
                // 5% → trả lời sai
                var correctIndex = LetterToIndex(session.CurrentQuestion.CorrectOption);
                var options = new List<int> { 0, 1, 2, 3 };
                options.Remove(correctIndex);
                answerIndex = options[rnd.Next(options.Count)];
            }
            else
            {
                // 5% → không trả lời, answerIndex = -1
            }

            if (answerIndex >= 0)
            {
                // Thời gian elapsed tính bằng giây
                double elapsedSec = delayMs / 1000.0;
                await SubmitAnswer(session.Id.ToString(), bot.Id.ToString(), (int)session.CurrentQuestion.Id, answerIndex, elapsedSec);
                Console.WriteLine($"🤖 Bot answered in {elapsedSec:F2}s - {(answerIndex == LetterToIndex(session.CurrentQuestion.CorrectOption) ? "correct" : "wrong")}");
            }
            else
            {
                Console.WriteLine($"🤖 Bot did not answer (timeout)");
            }
        }

    }


}

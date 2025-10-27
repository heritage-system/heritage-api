using Cultural_Heritage_System.Common;
using Cultural_Heritage_System.Dtos.Models;
using Cultural_Heritage_System.Dtos.Response.QuizQuestion;
using Cultural_Heritage_System.Models;
using Cultural_Heritage_System.Repositories;
using Cultural_Heritage_System.Services;
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

        private const int QUESTION_TIME = 10; // giây
        private const int READING_TIME = 3;
        // === DỮ LIỆU MẪU ===
        
        public GameHub(IHubContext<GameHub> hubContext, IQuizService quizService)
        {
            _hubContext = hubContext;
            _quizService = quizService;
        }

        // === GHÉP TRẬN ===
        public async Task FindMatch(string username, string avatarUrl)
        {
            var connectionId = Context.ConnectionId;

            // 🧩 Nếu đã tồn tại trong hàng chờ thì loại bỏ trước
            var existing = WaitingPlayers.FirstOrDefault(p => p.ConnectionId == connectionId);
            if (existing != null)
            {
                WaitingPlayers.Remove(existing);
                Console.WriteLine($"♻️ Removed old waiting record for {username}");
            }

            // 🧩 Kiểm tra nếu đang trong session cũ (bị mất đồng bộ)
            var inSession = Sessions.Values.FirstOrDefault(s => s.Players.Any(p => p.ConnectionId == connectionId));
            if (inSession != null)
            {
                Console.WriteLine($"⚠️ Player {username} already in session {inSession.Name}, cleaning up...");
                Sessions.Remove(Sessions.First(x => x.Value == inSession).Key);
            }

            var player = new Player
            {
                Id = Guid.NewGuid(),
                Username = username,
                AvatarUrl = avatarUrl,
                ConnectionId = connectionId
            };
            await Clients.Caller.SendAsync("WaitingForOpponent");
            // 🧩 Nếu đang chờ ai khác — ghép
            if (WaitingPlayers.Count > 0)
            {
                var waiting = WaitingPlayers.FirstOrDefault(p => p.ConnectionId != connectionId);
                if (waiting == null)
                {
                    // Không có ai khác -> thêm mới
                    WaitingPlayers.Add(new WaitingPlayer { ConnectionId = connectionId, Player = player });

                    _ = Task.Run(async () =>
                    {
                        try
                        {
                            var preQuestions = await _quizService.GenerateQuestionSet(10);
                            PreGeneratedQuestions[connectionId] = preQuestions;
                            Console.WriteLine($"🧩 Pre-generated questions for {username}");
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine($"⚠️ Failed to pre-generate questions for {username}: {ex.Message}");
                        }
                    });


                    
                    return;
                }

                WaitingPlayers.Remove(waiting);
                var roomId = Guid.NewGuid().ToString();

                var session = new GameSession
                {
                    Id = Guid.Parse(roomId),
                    Name = $"Match {roomId}",
                    Players = new List<Player> { waiting.Player, player },
                    Questions = PreGeneratedQuestions.ContainsKey(waiting.ConnectionId)
                                ? PreGeneratedQuestions[waiting.ConnectionId]
                                : await _quizService.GenerateQuestionSet(10),
                    CurrentQuestionIndex = 0
                };
                Sessions[roomId] = session;

                PreGeneratedQuestions.Remove(waiting.ConnectionId);
                PreGeneratedQuestions.Remove(connectionId);

                await Groups.AddToGroupAsync(waiting.ConnectionId, roomId);
                await Groups.AddToGroupAsync(connectionId, roomId);

                Console.WriteLine($"✅ Match created: {waiting.Player.Username} vs {player.Username} (room {roomId})");

                await Clients.Client(waiting.ConnectionId).SendAsync("MatchFound", roomId, session.Players);
                await Clients.Client(connectionId).SendAsync("MatchFound", roomId, session.Players);
                return;
            }

            // Không có ai chờ -> thêm mình vào
            WaitingPlayers.Add(new WaitingPlayer { ConnectionId = connectionId, Player = player });
            await Clients.Caller.SendAsync("WaitingForOpponent");
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
                if (useHubContext)
                   
                    await _hubContext.Clients.Group(roomId).SendAsync("GameFinished", session.Players);
                else
                    await Clients.Group(roomId).SendAsync("GameFinished", session.Players);
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

            // 🧹 Xóa session cũ (nếu có)
            var existingSession = Sessions.Values.FirstOrDefault(s => s.Players.Any(p => p.ConnectionId == connectionId));
            if (existingSession != null)
            {
                Sessions.Remove(Sessions.First(x => x.Value == existingSession).Key);
            }

            // 🧩 Tạo phòng mới
            var roomId = Guid.NewGuid().ToString("N").Substring(0, 6).ToUpper(); // ví dụ: "AB12CD"
            var player = new Player
            {
                Id = Guid.NewGuid(),
                Username = username,
                AvatarUrl = avatarUrl,
                ConnectionId = connectionId
            };

            var session = new GameSession
            {
                Id = Guid.NewGuid(),
                Name = $"Private Room {roomId}",
                Players = new List<Player> { player },
                Questions = await _quizService.GenerateQuestionSet(5),
                CurrentQuestionIndex = 0
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

            var player = new Player
            {
                Id = Guid.NewGuid(),
                Username = username,
                AvatarUrl = avatarUrl,
                ConnectionId = connectionId
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

    }

   

  
}

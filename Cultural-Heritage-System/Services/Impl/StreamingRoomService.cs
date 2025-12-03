using AutoMapper;
using Cultural_Heritage_System.Common;
using Cultural_Heritage_System.Dtos.Request.Streaming;
using Cultural_Heritage_System.Dtos.Response.Streaming;
using Cultural_Heritage_System.Middlewares;
using Cultural_Heritage_System.Models;
using Cultural_Heritage_System.Repositories;
using Microsoft.Extensions.Options;

namespace Cultural_Heritage_System.Services.Impl
{
    public class StreamingRoomService : IStreamingRoomService
    {
        private readonly IStreamingRoomRepository roomRepo;
        private readonly IEventRepository eventRepo;
        private readonly IStreamingParticipantRepository participantRepo;
        private readonly IUserRepository userRepo;
        private readonly IAgoraTokenService tokenSvc;
        private readonly AgoraOptions opt;
        private readonly IMapper mapper;
        private readonly IHttpContextAccessor http;

        public StreamingRoomService(
            IStreamingRoomRepository roomRepo,
            IEventRepository eventRepo,
            IStreamingParticipantRepository participantRepo,
            IUserRepository userRepo,
            IAgoraTokenService tokenSvc,
            IOptions<AgoraOptions> opt,
            IMapper mapper,
            IHttpContextAccessor http
        )
        {
            this.roomRepo = roomRepo;
            this.eventRepo = eventRepo;
            this.participantRepo = participantRepo;
            this.userRepo = userRepo;
            this.tokenSvc = tokenSvc;
            this.opt = opt.Value;
            this.mapper = mapper;
            this.http = http;
        }

        private int GetCurrentUserId()
        {
            var id = http.HttpContext?.User.FindFirst("userId")?.Value;
            if (string.IsNullOrWhiteSpace(id)) throw new AppException(ErrorCode.UNAUTHORIZED);
            return int.Parse(id);
        }

        private async Task EnsureHostOrCoHostAsync(int roomId, int userId)
        {
            var me = await participantRepo.GetByRoomAndUser(roomId, userId);
            if (me == null || (me.Role != RoomRole.HOST && me.Role != RoomRole.COHOST))
                throw new AppException(ErrorCode.FORBIDDEN);
        }

        private static string EnsureRtcUid(string? rtcUid, int userId)
            => string.IsNullOrWhiteSpace(rtcUid) ? userId.ToString() : rtcUid.Trim();

        public async Task<StreamingRoomResponse> CreateRoomAsync(StreamingRoomCreateRequest request)
        {
            var currentUserId = GetCurrentUserId();
            var creator = await userRepo.FindUserById(currentUserId)
                          ?? throw new AppException(ErrorCode.USER_NOT_EXISTED);

            Event? evt = null;
            if (request.EventId.HasValue)
            {
                // inject IEventRepository vào constructor
                evt = await eventRepo.GetByIdAsync(request.EventId.Value)
                      ?? throw new AppException(ErrorCode.EVENT_NOT_FOUND);
            }

            var room = new StreamingRoom
            {
                RoomName = $"room-{Guid.NewGuid():N}",
                Title = request.Title,
                CreatedByUserId = creator.Id,
                EventId = request.EventId,        // 🔥 gắn room vào Event
                IsActive = false,
                StartAt = request.StartAt,
                Type = StreamingRoomType.UPCOMING,
                ClosedAt = null
            };

            await roomRepo.AddAsync(room);

            // Người tạo phòng là Host
            await participantRepo.AddAsync(new StreamingParticipant
            {
                RoomId = room.Id,
                UserId = creator.Id,
                Role = RoomRole.HOST,
                Status = ParticipantStatus.WAITING,
                RtcUid = creator.Id.ToString()
            });

            return mapper.Map<StreamingRoomResponse>(room);
        }



        /// <summary>
        /// User đăng ký event (trước khi bắt đầu).
        /// </summary>
        public async Task RegisterAsync(string roomName)
        {
            var currentUserId = GetCurrentUserId();
            var room = await roomRepo.GetByRoomName(roomName)
                      ?? throw new AppException(ErrorCode.ROOM_NOT_FOUND);

            var exists = await participantRepo.GetByRoomAndUser(room.Id, currentUserId);
            if (exists != null) return;

            // dùng ParticipantStatus.Waiting như "Registered"
            await participantRepo.AddAsync(new StreamingParticipant
            {
                RoomId = room.Id,
                UserId = currentUserId,
                Role = RoomRole.AUDIENCE,
                Status = ParticipantStatus.WAITING,
                RtcUid = currentUserId.ToString()
            });
        }

        /// <summary>
        /// Cấp token join: auto tạo participant nếu chưa có, không cần admit/reject.
        /// </summary>
        public async Task<StreamingJoinGrantResponse> IssueJoinTokensAsync(string roomName)
        {
            var currentUserId = GetCurrentUserId();
            var room = await roomRepo.GetByRoomName(roomName)
                      ?? throw new AppException(ErrorCode.ROOM_NOT_FOUND);

            // phòng đã đóng thì cấm join luôn
            if (room.Type == StreamingRoomType.CLOSED)
                throw new AppException(ErrorCode.FORBIDDEN);

            var nowUtc = DateTime.UtcNow;

            // StartAt là DateTime thường => chuẩn hoá về UTC để so sánh
            var startAtUtc = room.StartAt.Kind == DateTimeKind.Utc
                ? room.StartAt
                : room.StartAt.ToUniversalTime();

            var sp = await participantRepo.GetByRoomAndUser(room.Id, currentUserId);
            var isCreator = room.CreatedByUserId == currentUserId;

            // 1️⃣ TRƯỚC GIỜ BẮT ĐẦU: ai join cũng chỉ là WAITING + bị chặn
            if (nowUtc < startAtUtc)
            {
                if (sp == null)
                {
                    sp = new StreamingParticipant
                    {
                        RoomId = room.Id,
                        UserId = currentUserId,
                        Role = isCreator ? RoomRole.HOST : RoomRole.AUDIENCE,
                        Status = ParticipantStatus.WAITING,
                        RtcUid = EnsureRtcUid(null, currentUserId)
                    };
                    await participantRepo.AddAsync(sp);
                }
                else
                {
                    if (sp.Status != ParticipantStatus.WAITING)
                    {
                        sp.Status = ParticipantStatus.WAITING;
                        if (string.IsNullOrWhiteSpace(sp.RtcUid))
                            sp.RtcUid = EnsureRtcUid(sp.RtcUid, currentUserId);

                        await participantRepo.UpdateAsync(sp);
                    }
                }

                // FE đang hiểu message ROOM_NOT_FOUND là "sự kiện chưa mở/phòng không tồn tại"
                throw new AppException(ErrorCode.ROOM_NOT_FOUND);
            }

            // 2️⃣ ĐÃ QUA GIỜ BẮT ĐẦU MÀ VẪN ĐANG UPCOMING
            if (room.Type == StreamingRoomType.UPCOMING)
            {
                if (!isCreator)
                {
                    // Audience tới sớm hơn host -> vẫn chỉ Waiting, không cho vào
                    if (sp == null)
                    {
                        sp = new StreamingParticipant
                        {
                            RoomId = room.Id,
                            UserId = currentUserId,
                            Role = RoomRole.AUDIENCE,
                            Status = ParticipantStatus.WAITING,
                            RtcUid = EnsureRtcUid(null, currentUserId)
                        };
                        await participantRepo.AddAsync(sp);
                    }
                    else if (sp.Status != ParticipantStatus.WAITING)
                    {
                        sp.Status = ParticipantStatus.WAITING;
                        await participantRepo.UpdateAsync(sp);
                    }

                    // Host chưa start phòng → coi như "chưa mở"
                    throw new AppException(ErrorCode.ROOM_NOT_FOUND);
                }

                // 👉 Host (creator) join lần đầu sau giờ start → chính thức mở phòng
                room.Type = StreamingRoomType.LIVE;
                room.IsActive = true;
                room.ClosedAt = null;
                await roomRepo.UpdateAsync(room);
            }

            // 3️⃣ Từ đây trở đi: chỉ xử lý phòng Live
            if (room.Type != StreamingRoomType.LIVE)
                throw new AppException(ErrorCode.ROOM_NOT_FOUND);

            if (sp != null && sp.Status == ParticipantStatus.KICKED)
                throw new AppException(ErrorCode.UNAUTHORIZED);

            if (sp == null)
            {
                sp = new StreamingParticipant
                {
                    RoomId = room.Id,
                    UserId = currentUserId,
                    RtcUid = EnsureRtcUid(null, currentUserId),
                    // Host đã có sẵn từ lúc create, path này thường chỉ cho audience
                    Role = RoomRole.AUDIENCE,
                    Status = ParticipantStatus.ADMITTED
                };
                await participantRepo.AddAsync(sp);
            }
            else
            {
                // chuyển sang Admitted nếu đang Waiting
                if (sp.Status != ParticipantStatus.ADMITTED)
                    sp.Status = ParticipantStatus.ADMITTED;

                if (string.IsNullOrWhiteSpace(sp.RtcUid))
                    sp.RtcUid = EnsureRtcUid(sp.RtcUid, currentUserId);

                await participantRepo.UpdateAsync(sp);
            }

            var rtcRole = (sp.Role is RoomRole.HOST or RoomRole.COHOST or RoomRole.SPEAKER)
                ? AgoraRtcRole.HOST
                : AgoraRtcRole.AUDIENCE;

            var (rtc, rtm) = await tokenSvc.CreateRteTokensAsync(
                room.RoomName, sp.RtcUid!, currentUserId.ToString(), rtcRole);

            var screenUid = $"{sp.RtcUid}-s";
            var screenRtc = await tokenSvc.CreateRtcTokenAsync(
                room.RoomName, screenUid, AgoraRtcRole.HOST);

            return new StreamingJoinGrantResponse
            {
                AppId = opt.AppId,
                Channel = room.RoomName,
                RtcUid = sp.RtcUid!,
                Role = sp.Role.ToString(),
                RtcToken = rtc,
                RtmToken = rtm,
                RtmUid = currentUserId.ToString(),
                ScreenRtcUid = screenUid,
                ScreenRtcToken = screenRtc
            };
        }





        public async Task<IReadOnlyList<StreamingParticipantResponse>> GetParticipantsAsync(
            string roomName, ParticipantStatus? status)
        {
            var room = await roomRepo.GetByRoomName(roomName)
                      ?? throw new AppException(ErrorCode.ROOM_NOT_FOUND);

            var list = await participantRepo.GetByRoom(room.Id, status);
            return list.Select(mapper.Map<StreamingParticipantResponse>).ToList();
        }

        public async Task<IReadOnlyList<StreamingRoomWithCountResponse>> GetRoomsHavingParticipantsAsync(
            int minCount = 1, ParticipantStatus? status = ParticipantStatus.ADMITTED)
        {
            var list = await roomRepo.GetRoomsHavingParticipantsAsync(minCount, status);
            return list.Select(x => new StreamingRoomWithCountResponse
            {
                Id = x.Room.Id,
                RoomName = x.Room.RoomName,
                Title = x.Room.Title ?? "",
                IsActive = x.Room.IsActive,
                CreatedAt = x.Room.CreatedAt,
                ParticipantCount = x.Count
            }).ToList();
        }

        public async Task HeartbeatAsync(string roomName)
        {
            var currentUserId = GetCurrentUserId();
            var room = await roomRepo.GetByRoomName(roomName)
                      ?? throw new AppException(ErrorCode.ROOM_NOT_FOUND);

            var sp = await participantRepo.GetByRoomAndUser(room.Id, currentUserId);
            if (sp == null || sp.Status == ParticipantStatus.KICKED)
                return;

            await participantRepo.TouchLastSeenAsync(room.Id, currentUserId);
        }

        public async Task LeaveAsync(string roomName)
        {
            var currentUserId = GetCurrentUserId();
            var room = await roomRepo.GetByRoomName(roomName)
                      ?? throw new AppException(ErrorCode.ROOM_NOT_FOUND);

            await participantRepo.MarkLeftAsync(room.Id, currentUserId, setStatusLeft: true);
        }

        public async Task SetRoleAsync(string roomName, StreamingSetRoleRequest request)
        {
            var currentUserId = GetCurrentUserId();
            var room = await roomRepo.GetByRoomName(roomName)
                      ?? throw new AppException(ErrorCode.ROOM_NOT_FOUND);

            await EnsureHostOrCoHostAsync(room.Id, currentUserId);

            var sp = await participantRepo.GetByRoomAndUser(room.Id, request.UserId)
                     ?? throw new AppException(ErrorCode.ROOM_NOT_FOUND);

            sp.Role = request.Role;
            await participantRepo.UpdateAsync(sp);
        }

        public async Task KickAsync(string roomName, StreamingAdmitRejectRequest request)
        {
            var currentUserId = GetCurrentUserId();
            var room = await roomRepo.GetByRoomName(roomName)
                      ?? throw new AppException(ErrorCode.ROOM_NOT_FOUND);

            await EnsureHostOrCoHostAsync(room.Id, currentUserId);

            var sp = await participantRepo.GetByRoomAndUser(room.Id, request.UserId)
                     ?? throw new AppException(ErrorCode.USER_NOT_EXISTED);

            sp.Status = ParticipantStatus.KICKED;
            await participantRepo.UpdateAsync(sp);
        }
        public async Task<IReadOnlyList<StreamingRoomResponse>> GetUpcomingRoomsAsync(DateTime? from = null)
        {
            var fromUtc = from?.ToUniversalTime() ?? DateTime.UtcNow;
            var list = await roomRepo.GetUpcomingRoomsAsync(fromUtc, max: 20);
            return list.Select(mapper.Map<StreamingRoomResponse>).ToList();
        }
        public async Task<IReadOnlyList<StreamingRoomResponse>> GetRoomsAdminAsync(StreamingRoomType? type = null)
        {
            // Tùy bạn: check role Admin ở đây hoặc bằng [Authorize(Roles="Admin")] ở controller
            var rooms = await roomRepo.GetRooms(page: 1, size: int.MaxValue); // đơn giản, không phân trang
            if (type.HasValue)
            {
                rooms = rooms.Where(r => r.Type == type.Value).ToList();
            }

            return rooms.Select(mapper.Map<StreamingRoomResponse>).ToList();
        }
        public async Task<StreamingRoomDetailResponse> GetRoomDetailAsync(string roomName)
        {
            var room = await roomRepo.GetByRoomName(roomName)
                      ?? throw new AppException(ErrorCode.ROOM_NOT_FOUND);

            var parts = await participantRepo.GetByRoom(room.Id, status: null);

            var dto = mapper.Map<StreamingRoomDetailResponse>(room);
            dto.Participants = parts.Select(mapper.Map<StreamingParticipantResponse>).ToList();

            return dto;
        }
        public async Task<StreamingRoomResponse> UpdateRoomAsync(string roomName, StreamingRoomUpdateRequest request)
        {
            var room = await roomRepo.GetByRoomName(roomName)
                      ?? throw new AppException(ErrorCode.ROOM_NOT_FOUND);

            if (!string.IsNullOrWhiteSpace(request.Title))
            {
                room.Title = request.Title.Trim();
            }

            if (request.StartAt.HasValue)
            {
                room.StartAt = request.StartAt.Value;
            }

            if (request.Type.HasValue)
            {
                var newType = request.Type.Value;

                room.Type = newType;
                switch (newType)
                {
                    case StreamingRoomType.UPCOMING:
                        room.IsActive = false;
                        room.ClosedAt = null;
                        break;

                    case StreamingRoomType.LIVE:
                        room.IsActive = true;
                        room.ClosedAt = null;
                        break;

                    case StreamingRoomType.CLOSED:
                        room.IsActive = false;
                        room.ClosedAt = DateTime.UtcNow;
                        break;
                }
            }

            await roomRepo.UpdateAsync(room);
            return mapper.Map<StreamingRoomResponse>(room);
        }
        public async Task DeleteRoomAsync(string roomName)
        {
            var room = await roomRepo.GetByRoomName(roomName)
                      ?? throw new AppException(ErrorCode.ROOM_NOT_FOUND);

            await roomRepo.DeleteAsync(room);
        }
        public async Task<StreamingJoinGrantResponse> IssueAdminJoinAsCoHostAsync(string roomName)
        {
            var currentUserId = GetCurrentUserId();
            var room = await roomRepo.GetByRoomName(roomName)
                      ?? throw new AppException(ErrorCode.ROOM_NOT_FOUND);

            // Tùy bạn check role admin ở đây (ví dụ claim "role" == "Admin")

            var sp = await participantRepo.GetByRoomAndUser(room.Id, currentUserId);

            if (sp == null)
            {
                sp = new StreamingParticipant
                {
                    RoomId = room.Id,
                    UserId = currentUserId,
                    RtcUid = EnsureRtcUid(null, currentUserId),
                    Role = RoomRole.COHOST,
                    Status = ParticipantStatus.ADMITTED
                };
                await participantRepo.AddAsync(sp);
            }
            else
            {
                sp.Role = RoomRole.COHOST;
                sp.Status = ParticipantStatus.ADMITTED;
                if (string.IsNullOrWhiteSpace(sp.RtcUid))
                    sp.RtcUid = EnsureRtcUid(sp.RtcUid, currentUserId);

                await participantRepo.UpdateAsync(sp);
            }

            var rtcRole = AgoraRtcRole.HOST; // CoHost cũng là publisher
            var (rtc, rtm) = await tokenSvc.CreateRteTokensAsync(
                room.RoomName, sp.RtcUid!, currentUserId.ToString(), rtcRole);

            var screenUid = $"{sp.RtcUid}-s";
            var screenRtc = await tokenSvc.CreateRtcTokenAsync(
                room.RoomName, screenUid, AgoraRtcRole.HOST);

            return new StreamingJoinGrantResponse
            {
                AppId = opt.AppId,
                Channel = room.RoomName,
                RtcUid = sp.RtcUid!,
                Role = sp.Role.ToString(),
                RtcToken = rtc,
                RtmToken = rtm,
                RtmUid = currentUserId.ToString(),
                ScreenRtcUid = screenUid,
                ScreenRtcToken = screenRtc
            };
        }

        public async Task<IReadOnlyList<StreamingRoomResponse>> GetRoomsByEventAsync(long eventId)
        {
            var evt = await eventRepo.GetEventByIdAsync(eventId)
                      ?? throw new AppException(ErrorCode.EVENT_NOT_FOUND);


            var rooms = (evt.StreamingRooms ?? new List<StreamingRoom>())
                .OrderBy(r => r.StartAt)
                .ToList();

            return rooms.Select(mapper.Map<StreamingRoomResponse>).ToList();
        }
    }
}

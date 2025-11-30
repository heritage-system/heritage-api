// Services/Impl/StreamingRoomService.cs
using AutoMapper;
using Cultural_Heritage_System.Common;
using Cultural_Heritage_System.Dtos.Request.Streaming;
using Cultural_Heritage_System.Dtos.Response.Streaming;
using Cultural_Heritage_System.Helpers;
using Cultural_Heritage_System.Middlewares;
using Cultural_Heritage_System.Models;
using Cultural_Heritage_System.Repositories;
using Microsoft.Extensions.Options;

namespace Cultural_Heritage_System.Services.Impl
{
    public class StreamingRoomService : IStreamingRoomService
    {
        private readonly IStreamingRoomRepository roomRepo;
        private readonly IStreamingParticipantRepository participantRepo;
        private readonly IRaiseHandRepository raiseRepo;
        private readonly IRoomChatRepository chatRepo;
        private readonly IUserRepository userRepo;
        private readonly IAgoraTokenService tokenSvc;
        private readonly AgoraOptions opt;
        private readonly IMapper mapper;
        private readonly IHttpContextAccessor http;   // <— thêm
        private readonly StreamAdmissionOptions admissionOpt; // <- new
        public StreamingRoomService(
       IStreamingRoomRepository roomRepo,
       IStreamingParticipantRepository participantRepo,
       IRaiseHandRepository raiseRepo,
       IRoomChatRepository chatRepo,
       IUserRepository userRepo,
       IAgoraTokenService tokenSvc,
       IOptions<AgoraOptions> opt,
       IMapper mapper,
       IHttpContextAccessor http,
       IOptions<StreamAdmissionOptions> admissionOpt // <- new
   )
        {
            this.roomRepo = roomRepo;
            this.participantRepo = participantRepo;
            this.raiseRepo = raiseRepo;
            this.chatRepo = chatRepo;
            this.userRepo = userRepo;
            this.tokenSvc = tokenSvc;
            this.opt = opt.Value;
            this.mapper = mapper;
            this.http = http;
            this.admissionOpt = admissionOpt.Value; // <- new
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
            if (me == null || (me.Role != RoomRole.Host && me.Role != RoomRole.CoHost))
                throw new AppException(ErrorCode.FORBIDDEN);
        }

        public async Task<StreamingRoomResponse> CreateRoomAsync(StreamingRoomCreateRequest request)
        {
            var currentUserId = GetCurrentUserId();
            var creator = await userRepo.FindUserById(currentUserId)
                          ?? throw new AppException(ErrorCode.USER_NOT_EXISTED);

            var room = new StreamingRoom
            {
                RoomName = $"room-{Guid.NewGuid():N}",
                Title = request.Title,
                CreatedByUserId = creator.Id,
                IsActive = true
            };
            await roomRepo.AddAsync(room);

            await participantRepo.AddAsync(new StreamingParticipant
            {
                RoomId = room.Id,
                UserId = creator.Id,
                Role = RoomRole.Host,
                Status = ParticipantStatus.Admitted,
                RtcUid = creator.Id.ToString()
            });

            await chatRepo.AddAsync(new RoomChatMessage
            {
                RoomId = room.Id,
                IsSystem = true,
                Content = $"Room created by {creator.UserName}"
            });

            return mapper.Map<StreamingRoomResponse>(room);
        }
        private static string EnsureRtcUid(string? rtcUid, int userId)
       => string.IsNullOrWhiteSpace(rtcUid) ? userId.ToString() : rtcUid.Trim();
        public async Task RequestJoinAsync(string roomName, StreamingRequestJoinRequest request)
        {
            var currentUserId = GetCurrentUserId();
            var room = await roomRepo.GetByRoomName(roomName)
                      ?? throw new AppException(ErrorCode.ROOM_NOT_FOUND);
            var user = await userRepo.FindUserById(currentUserId)
                      ?? throw new AppException(ErrorCode.USER_NOT_EXISTED);

            var sp = await participantRepo.GetByRoomAndUser(room.Id, user.Id);
            if (admissionOpt.OpenAdmission)
            {
                // ✅ Open admission: tạo mới nếu chưa có, đặt Admitted luôn, KHÔNG tạo RaiseHand
                if (sp == null)
                {
                    await participantRepo.AddAsync(new StreamingParticipant
                    {
                        RoomId = room.Id,
                        UserId = user.Id,
                        Status = ParticipantStatus.Admitted,
                        Role = RoomRole.Audience,
                        RtcUid = EnsureRtcUid(request.RtcUid, user.Id)
                    });
                }
                else
                {
                    sp.Status = ParticipantStatus.Admitted;
                    sp.RtcUid = EnsureRtcUid(sp.RtcUid ?? request.RtcUid, user.Id);
                    await participantRepo.UpdateAsync(sp);
                }

                await chatRepo.AddAsync(new RoomChatMessage
                {
                    RoomId = room.Id,
                    IsSystem = true,
                    Content = $"[Auto-admit] User {user.Id} joined."
                });

                return; // 🚪 xong, không tạo RaiseHand
            }

            // ❄️ Legacy (nếu sau này muốn tắt open admission)
            if (sp == null)
            {
                await participantRepo.AddAsync(new StreamingParticipant
                {
                    RoomId = room.Id,
                    UserId = user.Id,
                    Status = ParticipantStatus.Waiting,
                    Role = RoomRole.Audience,
                    RtcUid = EnsureRtcUid(request.RtcUid, user.Id)
                });
            }
            await raiseRepo.AddAsync(new RaiseHandRequest
            {
                RoomId = room.Id,
                UserId = user.Id,
                Status = RaiseHandStatus.Pending
            });
        }

        public async Task AdmitAsync(string roomName, StreamingAdmitRejectRequest request)
        {
            if (admissionOpt.OpenAdmission)
            {
                // ✅ Không dùng nữa – có thể no-op an toàn
                return;
            }
            var currentUserId = GetCurrentUserId();
            var room = await roomRepo.GetByRoomName(roomName)
                      ?? throw new AppException(ErrorCode.ROOM_NOT_FOUND);

            await EnsureHostOrCoHostAsync(room.Id, currentUserId);

            var sp = await participantRepo.GetByRoomAndUser(room.Id, request.UserId)
                     ?? throw new AppException(ErrorCode.ROOM_NOT_FOUND);

            sp.Status = ParticipantStatus.Admitted;
            sp.IsRaisedHand = false;
            await participantRepo.UpdateAsync(sp);

            var pending = await raiseRepo.GetPending(room.Id);
            foreach (var r in pending.Where(r => r.UserId == request.UserId))
            {
                r.Status = RaiseHandStatus.Approved;
                r.ResolvedAt = DateTime.UtcNow;
                await raiseRepo.UpdateAsync(r);
            }

            await chatRepo.AddAsync(new RoomChatMessage
            {
                RoomId = room.Id,
                IsSystem = true,
                Content = $"User {request.UserId} admitted."
            });
        }
        public async Task HeartbeatAsync(string roomName)
        {
            var currentUserId = GetCurrentUserId();
            var room = await roomRepo.GetByRoomName(roomName) ?? throw new AppException(ErrorCode.ROOM_NOT_FOUND);

            var sp = await participantRepo.GetByRoomAndUser(room.Id, currentUserId);

            if (admissionOpt.OpenAdmission && sp == null)
            {
                await participantRepo.AddAsync(new StreamingParticipant
                {
                    RoomId = room.Id,
                    UserId = currentUserId,
                    Status = ParticipantStatus.Admitted,
                    Role = RoomRole.Audience,
                    RtcUid = currentUserId.ToString(),
                    LastSeenAt = DateTime.UtcNow
                });
                return;
            }

            if (sp == null || sp.Status == ParticipantStatus.Kicked)
                return; // ❌ không gia hạn/bật lại cho user đã bị kick

            await participantRepo.TouchLastSeenAsync(room.Id, currentUserId);
        }

        public async Task LeaveAsync(string roomName)
        {
            var currentUserId = GetCurrentUserId();
            var room = await roomRepo.GetByRoomName(roomName) ?? throw new AppException(ErrorCode.ROOM_NOT_FOUND);

            await participantRepo.MarkLeftAsync(room.Id, currentUserId, setStatusLeft: true);

            await chatRepo.AddAsync(new RoomChatMessage
            {
                RoomId = room.Id,
                IsSystem = true,
                Content = $"User {currentUserId} left the room."
            });
        }
        public async Task RejectAsync(string roomName, StreamingAdmitRejectRequest request)
        {
            if (admissionOpt.OpenAdmission)
            {
                // ✅ Không dùng nữa – no-op
                return;
            }
            var currentUserId = GetCurrentUserId();
            var room = await roomRepo.GetByRoomName(roomName)
                      ?? throw new AppException(ErrorCode.ROOM_NOT_FOUND);

            await EnsureHostOrCoHostAsync(room.Id, currentUserId);

            var sp = await participantRepo.GetByRoomAndUser(room.Id, request.UserId)
                     ?? throw new AppException(ErrorCode.ROOM_NOT_FOUND);

            sp.Status = ParticipantStatus.Kicked;
            sp.IsRaisedHand = false;
            await participantRepo.UpdateAsync(sp);

            var pending = await raiseRepo.GetPending(room.Id);
            foreach (var r in pending.Where(r => r.UserId == request.UserId))
            {
                r.Status = RaiseHandStatus.Rejected;
                r.ResolvedAt = DateTime.UtcNow;
                await raiseRepo.UpdateAsync(r);
            }
        }


        public async Task SetRoleAsync(string roomName, StreamingSetRoleRequest request)
        {
            var currentUserId = GetCurrentUserId();
            var room = await roomRepo.GetByRoomName(roomName)
                      ?? throw new AppException(ErrorCode.ROOM_NOT_FOUND);

            await EnsureHostOrCoHostAsync(room.Id, currentUserId);

            var sp = await participantRepo.GetByRoomAndUser(room.Id, request.UserId)
                     ?? throw new AppException(ErrorCode.ROOM_NOT_FOUND);

            sp.Role = request.Role; // ✅ enum đã bind từ string nhờ JsonStringEnumConverter
            await participantRepo.UpdateAsync(sp);

            await chatRepo.AddAsync(new RoomChatMessage
            {
                RoomId = room.Id,
                IsSystem = true,
                Content = $"User {request.UserId} role -> {request.Role}"
            });
        }
        public async Task RaiseHandAsync(string roomName, StreamingRaiseHandRequest request)
        {

            var currentUserId = GetCurrentUserId();

            var room = await roomRepo.GetByRoomName(roomName)
                      ?? throw new AppException(ErrorCode.ROOM_NOT_FOUND);
            var sp = await participantRepo.GetByRoomAndUser(room.Id, currentUserId)
                     ?? throw new AppException(ErrorCode.ROOM_NOT_FOUND);
            sp.IsRaisedHand = request.Raised;
            if (admissionOpt.OpenAdmission)
            {
                // ✅ Open admission: giơ tay không cần thiết để vào phòng.
                // Nếu bạn vẫn muốn dùng giơ tay làm tín hiệu xin phát biểu thì cứ giữ DB như cũ.
                // Ở đây tối giản → chỉ tắt việc ghi RaiseHand để giảm tải:

                await participantRepo.UpdateAsync(sp);
                return;
            }
            if (request.Raised)
            {
                await raiseRepo.AddAsync(new RaiseHandRequest
                {
                    RoomId = room.Id,
                    UserId = currentUserId,
                    Status = RaiseHandStatus.Pending
                });
            }
        }
        public async Task<IReadOnlyList<StreamingRoomWithCountResponse>> GetRoomsHavingParticipantsAsync(
            int minCount = 1, ParticipantStatus? status = ParticipantStatus.Admitted)
        {
            var list = await roomRepo.GetRoomsHavingParticipantsAsync(minCount, status);

            // map thủ công để đưa Count vào DTO
            var result = list.Select(x => new StreamingRoomWithCountResponse
            {
                Id = x.Room.Id,
                RoomName = x.Room.RoomName,
                Title = x.Room.Title ?? "",
                IsActive = x.Room.IsActive,
                CreatedAt = x.Room.CreatedAt,
                ParticipantCount = x.Count
            }).ToList();

            return result;
        }
        public async Task<IReadOnlyList<StreamingParticipantResponse>> GetParticipantsAsync(string roomName, ParticipantStatus? status)
        {
            var room = await roomRepo.GetByRoomName(roomName) ?? throw new AppException(ErrorCode.ROOM_NOT_FOUND);
            var list = await participantRepo.GetByRoom(room.Id, status);
            return list.Select(mapper.Map<StreamingParticipantResponse>).ToList();
        }

        public async Task<StreamingJoinGrantResponse> IssueJoinTokensAsync(string roomName)
        {
            var currentUserId = GetCurrentUserId();
            var room = await roomRepo.GetByRoomName(roomName)
                      ?? throw new AppException(ErrorCode.ROOM_NOT_FOUND);

            var sp = await participantRepo.GetByRoomAndUser(room.Id, currentUserId);
            if (sp != null && sp.Status == ParticipantStatus.Kicked)
            {
                // KHÔNG cấp token cho user bị kick
                throw new AppException(ErrorCode.UNAUTHORIZED); // hoặc ErrorCode.FORBIDDEN
            }
            if (admissionOpt.OpenAdmission)
            {
                // ✅ auto create or auto-upgrade lên Admitted
                if (sp == null)
                {
                    sp = new StreamingParticipant
                    {
                        RoomId = room.Id,
                        UserId = currentUserId,
                        RtcUid = currentUserId.ToString(),
                        Role = RoomRole.Audience,
                        Status = ParticipantStatus.Admitted
                    };
                    await participantRepo.AddAsync(sp);
                }
                else if (sp.Status != ParticipantStatus.Admitted)
                {
                    sp.Status = ParticipantStatus.Admitted;
                    if (string.IsNullOrWhiteSpace(sp.RtcUid)) sp.RtcUid = currentUserId.ToString();
                    await participantRepo.UpdateAsync(sp);
                }
            }
            else
            {
                if (sp == null || sp.Status != ParticipantStatus.Admitted)
                    throw new AppException(ErrorCode.UNAUTHORIZED);
            }

            var rtcRole = (sp.Role is RoomRole.Host or RoomRole.CoHost or RoomRole.Speaker)
                            ? AgoraRtcRole.HOST
                            : AgoraRtcRole.AUDIENCE;

            var (rtc, rtm) = await tokenSvc.CreateRteTokensAsync(
                room.RoomName, sp.RtcUid!, currentUserId.ToString(), rtcRole);

            // NEW: token riêng cho screen-client
            var screenUid = $"{sp.RtcUid}-s";
            var screenRtc = await tokenSvc.CreateRtcTokenAsync(
                room.RoomName, screenUid, AgoraRtcRole.HOST); // screen nên là HOST để publish video

            return new StreamingJoinGrantResponse
            {
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


        public async Task<IReadOnlyList<StreamingParticipantResponse>> GetWaitingListAsync(string roomName)
        {
            if (admissionOpt.OpenAdmission)
            {
                // ✅ Open admission: không chạm DB để đỡ query → trả về rỗng
                return Array.Empty<StreamingParticipantResponse>();
            }

            var currentUserId = GetCurrentUserId();
            var room = await roomRepo.GetByRoomName(roomName)
                      ?? throw new AppException(ErrorCode.ROOM_NOT_FOUND);
            await EnsureHostOrCoHostAsync(room.Id, currentUserId);

            var waits = await participantRepo.GetWaitingList(room.Id);
            return waits.Select(mapper.Map<StreamingParticipantResponse>).ToList();
        }

        public async Task KickAsync(string roomName, StreamingAdmitRejectRequest request)
        {
            var currentUserId = GetCurrentUserId();
            var room = await roomRepo.GetByRoomName(roomName)
                      ?? throw new AppException(ErrorCode.ROOM_NOT_FOUND);

            await EnsureHostOrCoHostAsync(room.Id, currentUserId);

            var sp = await participantRepo.GetByRoomAndUser(room.Id, request.UserId)
                     ?? throw new AppException(ErrorCode.USER_NOT_EXISTED);

            sp.Status = ParticipantStatus.Kicked;
            sp.IsRaisedHand = false;
            await participantRepo.UpdateAsync(sp);

            await chatRepo.AddAsync(new RoomChatMessage
            {
                RoomId = room.Id,
                IsSystem = true,
                Content = $"User {request.UserId} kicked by {currentUserId}."
            });
        }

    }
}

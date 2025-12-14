using AutoMapper;
using Cultural_Heritage_System.Common;
using Cultural_Heritage_System.Dtos.Request.Event;
using Cultural_Heritage_System.Dtos.Response;
using Cultural_Heritage_System.Dtos.Response.Event;
using Cultural_Heritage_System.Dtos.Response.EventRegistration;
using Cultural_Heritage_System.Dtos.Response.GameMatchHistory;
using Cultural_Heritage_System.Helpers;
using Cultural_Heritage_System.Middlewares;
using Cultural_Heritage_System.Models;
using Cultural_Heritage_System.Repositories;
using Cultural_Heritage_System.Repositories.Impl;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using static StackExchange.Redis.Role;

namespace Cultural_Heritage_System.Services.Impl
{
    public class EventService : IEventService
    {
        private readonly IHttpContextAccessor _http;
        private readonly IEventRepository _eventRepo;
        private readonly IEventRegistrationRepository _regRepo;
        private readonly IStreamingParticipantRepository _participantRepo;
        private readonly IMapper _mapper;
        private readonly IMailService _mailService;   // 👈 THÊM

        public EventService(
            IEventRepository eventRepo,
            IEventRegistrationRepository regRepo,
            IStreamingParticipantRepository participantRepo,
            IMapper mapper,
            IHttpContextAccessor http,
            IMailService mailService)                 // 👈 THÊM
        {
            _eventRepo = eventRepo;
            _regRepo = regRepo;
            _participantRepo = participantRepo;
            _mapper = mapper;
            _http = http;
            _mailService = mailService;              // 👈 THÊM
        }


        private int GetCurrentUserId()
        {
            var accountIdClaim = _http.HttpContext?.User.FindFirst("userId")?.Value;
            if (string.IsNullOrEmpty(accountIdClaim))
            {
                throw new AppException(ErrorCode.UNAUTHORIZED);
            }

            return int.Parse(accountIdClaim);
        }

        private int? TryGetUserId()
        {
            var id = _http.HttpContext?.User.FindFirst("userId")?.Value;
            return string.IsNullOrWhiteSpace(id) ? (int?)null : int.Parse(id);
        }


        private async Task ScheduleRemindEmailsForRoomAsync(Event ev, StreamingRoom room)
        {
            // Lấy toàn bộ đăng ký (kèm User)
            var regs = await _regRepo.GetByEventWithUserAsync(ev.Id);
            if (regs == null || regs.Count == 0) return;

            // Chuẩn hoá thời gian start của room
            var roomStartUtc = room.StartAt.Kind == DateTimeKind.Utc
                ? room.StartAt
                : room.StartAt.ToUniversalTime();

            // Thời điểm gửi email: 5 phút trước giờ start
            var scheduleUtc = roomStartUtc.AddMinutes(-5);

            // Nếu đã quá muộn (start < 5'), thì đẩy lên ~1 phút sau hiện tại cho an toàn
            if (scheduleUtc <= DateTime.UtcNow)
                scheduleUtc = DateTime.UtcNow.AddMinutes(1);

            // Format giờ/ngày hiển thị cho user (theo local server)
            var local = roomStartUtc.ToLocalTime();
            var startTimeStr = local.ToString("HH:mm");
            var eventDateStr = local.ToString("dd/MM/yyyy");

            foreach (var reg in regs.Where(r => !r.IsCancelled))
            {
                var user = reg.User;
                if (user == null || string.IsNullOrWhiteSpace(user.Email))
                    continue;

                var userName = user.Profile?.FullName ?? user.UserName ?? user.Email;

                // TODO: chỉnh cho đúng route FE của bạn
                var joinUrl = $"https://heritage-web-ashy.vercel.app/live/{room.RoomName}";


                await _mailService.SendRemindEmail(
                    user.Email,
                    userName,
                    ev.Title,
                    startTimeStr,
                    eventDateStr,
                    joinUrl,
                    scheduleUtc);
            }
        }



        public async Task DeleteEventAsync(long id)
        {
            var e = await _eventRepo.GetByIdAsync(id)
                    ?? throw new AppException(ErrorCode.EVENT_NOT_FOUND);

            await _eventRepo.DeleteAsync(e);
        }

        public async Task<List<EventResponse>> GetEventsAsync(EventStatus? status, DateTime? from = null)
        {
            var q = _eventRepo.GetEventsWithIncludes();

            if (status.HasValue)
                q = q.Where(e => e.Status == status.Value);

            if (from.HasValue)
            {
                var f = from.Value.ToUniversalTime();
                q = q.Where(e => e.StartAt >= f);
            }

            var list = await q.OrderBy(e => e.StartAt).ToListAsync();
            var currentUserId = TryGetUserId();

            return list.Select(e =>
            {
                var dto = _mapper.Map<EventResponse>(e);
                dto.RegisteredByMe = currentUserId.HasValue &&
                                     e.Registrations.Any(r => r.UserId == currentUserId && !r.IsCancelled);
                return dto;
            }).ToList();
        }

        public async Task<EventResponse> GetEventDetailAsync(long id)
        {
            var e = await _eventRepo.GetEventByIdAsync(id)
                    ?? throw new AppException(ErrorCode.EVENT_NOT_FOUND);

            var currentUserId = TryGetUserId();
            var dto = _mapper.Map<EventResponse>(e);
            dto.RegisteredByMe = currentUserId.HasValue &&
                e.Registrations.Any(r => r.UserId == currentUserId && !r.IsCancelled);

            return dto;
        }

        public async Task<EventRegistrationResponse> RegisterAsync(long eventId)
        {
            var userId = GetCurrentUserId();
            var e = await _eventRepo.GetByIdAsync(eventId)
                    ?? throw new AppException(ErrorCode.EVENT_NOT_FOUND);

            if (e.Status is EventStatus.CLOSED or EventStatus.ARCHIVED)
                throw new AppException(ErrorCode.EVENT_CLOSED);

            var existing = await _regRepo.GetByEventAndUserAsync(eventId, userId);
            if (existing != null)
            {
                if (existing.IsCancelled)
                {
                    existing.IsCancelled = false;
                    existing.RegisteredAt = DateTime.UtcNow;
                    await _regRepo.UpdateAsync(existing);
                }
            }
            else
            {
                var reg = new EventRegistration
                {
                    EventId = eventId,
                    UserId = userId,
                    RegisteredAt = DateTime.UtcNow,
                    IsCancelled = false
                };
                await _regRepo.AddAsync(reg);
            }

            return new EventRegistrationResponse
            {
                EventId = eventId,
                Registered = true
            };
        }

        public async Task<EventRegistrationResponse> UnregisterAsync(long eventId)
        {
            var userId = GetCurrentUserId();
            var existing = await _regRepo.GetByEventAndUserAsync(eventId, userId);
            if (existing != null && !existing.IsCancelled)
            {
                existing.IsCancelled = true;
                await _regRepo.UpdateAsync(existing);
            }

            return new EventRegistrationResponse
            {
                EventId = eventId,
                Registered = false
            };
        }
        public async Task<List<EventRegistrationUserResponse>> GetEventRegistrationsWithUserAsync(long eventId)
        {
            // đảm bảo event tồn tại
            var ev = await _eventRepo.GetByIdAsync(eventId)
                     ?? throw new AppException(ErrorCode.EVENT_NOT_FOUND);

            var regs = await _regRepo.GetByEventWithUserAsync(eventId);

            return regs
                .Select(r => _mapper.Map<EventRegistrationUserResponse>(r))
                .ToList();
        }
        public async Task<EventResponse> CreateEventWithRoomsAsync(EventWithRoomsCreateRequest request)
        {
            var userId = GetCurrentUserId();

            // ---- Map Event entity ----
            var entity = new Event
            {
                Title = request.Title.Trim(),
                Description = request.Description,
                ThumbnailUrl = request.ThumbnailUrl,
                StartAt = request.StartAt,
                CloseAt = request.CloseAt,
                Category = request.Category,
                Tags = request.Tags,
                CreatedBy = userId.ToString(),
            };

            // ---- Tính status dựa trên thời gian ----
            var nowUtc = DateTime.UtcNow;

            var startUtc = request.StartAt.Kind == DateTimeKind.Utc
                ? request.StartAt
                : request.StartAt.ToUniversalTime();

            DateTime? closeUtc = null;
            if (request.CloseAt.HasValue)
            {
                closeUtc = request.CloseAt.Value.Kind == DateTimeKind.Utc
                    ? request.CloseAt.Value
                    : request.CloseAt.Value.ToUniversalTime();
            }

            if (closeUtc.HasValue && closeUtc.Value <= nowUtc)
            {
                entity.Status = EventStatus.CLOSED;
            }
            else if (startUtc > nowUtc)
            {
                entity.Status = EventStatus.UPCOMING;
            }
            else
            {
                entity.Status = EventStatus.LIVE;
            }

            // ---- Tạo list StreamingRoom nếu FE gửi lên ----
            if (request.Rooms != null && request.Rooms.Count > 0)
            {
                entity.StreamingRooms = request.Rooms.Select(rDto =>
                {
                    var roomStart = rDto.StartAt ?? request.StartAt;
                    var type = rDto.Type ?? StreamingRoomType.UPCOMING;

                    return new StreamingRoom
                    {
                        RoomName = $"room-{Guid.NewGuid():N}",
                        Title = string.IsNullOrWhiteSpace(rDto.Title)
                            ? entity.Title
                            : rDto.Title.Trim(),
                        CreatedBy = userId.ToString(),
                        StartAt = roomStart,
                        Type = type,
                        IsActive = type == StreamingRoomType.LIVE,
                        ClosedAt = type == StreamingRoomType.CLOSED ? DateTime.UtcNow : null
                    };
                }).ToList();
            }

            // Save Event + Rooms
            await _eventRepo.AddAsync(entity);

            // ---- Tạo host participant cho từng room mới ----
            if (entity.StreamingRooms != null && entity.StreamingRooms.Count > 0)
            {
                foreach (var room in entity.StreamingRooms)
                {
                    await _participantRepo.AddAsync(new StreamingParticipant
                    {
                        RoomId = room.Id,
                        UserId = userId,
                        Role = RoomRole.HOST,
                        Status = ParticipantStatus.WAITING,
                        RtcUid = userId.ToString()
                    });
                }
            }

            var created = await _eventRepo.GetEventByIdAsync(entity.Id)
                          ?? throw new AppException(ErrorCode.EVENT_NOT_FOUND);

            var resp = _mapper.Map<EventResponse>(created);
            resp.RegisteredByMe = false;
            return resp;
        }

        public async Task<EventResponse> UpdateEventWithRoomsAsync(EventWithRoomsUpdateRequest request)
        {
            var e = await _eventRepo.GetEventByIdAsync(request.Id)
                    ?? throw new AppException(ErrorCode.EVENT_NOT_FOUND);

            // ---- Update Event ----
            if (!string.IsNullOrWhiteSpace(request.Title))
                e.Title = request.Title.Trim();

            if (request.Description != null)
                e.Description = request.Description;

            if (request.ThumbnailUrl != null)
                e.ThumbnailUrl = request.ThumbnailUrl;

            if (request.StartAt.HasValue)
                e.StartAt = request.StartAt.Value;

            if (request.CloseAt.HasValue)
                e.CloseAt = request.CloseAt.Value;

            if (request.Category.HasValue)
                e.Category = request.Category.Value;

            if (request.Tags.HasValue)
                e.Tags = request.Tags.Value;

            // Status: nếu FE gửi thì dùng, không thì có thể auto tính lại nếu giờ thay đổi
            if (request.Status.HasValue)
            {
                e.Status = request.Status.Value;
            }
            else if (request.StartAt.HasValue || request.CloseAt.HasValue)
            {
                var nowUtc = DateTime.UtcNow;
                var startUtc = e.StartAt.Kind == DateTimeKind.Utc
                    ? e.StartAt
                    : e.StartAt.ToUniversalTime();
                var closeUtc = e.CloseAt?.ToUniversalTime();

                if (closeUtc.HasValue && closeUtc <= nowUtc)
                    e.Status = EventStatus.CLOSED;
                else if (startUtc > nowUtc)
                    e.Status = EventStatus.UPCOMING;
                else
                    e.Status = EventStatus.LIVE;
            }

            // ---- Upsert danh sách room ----
            // room hiện có trong DB
            var existingRooms = (e.StreamingRooms ?? new List<StreamingRoom>()).ToList();
            var existingById = existingRooms.ToDictionary(r => r.Id, r => r);

            // Lấy id người tạo từ Event.CreatedBy (string), nếu fail thì fallback sang current user

            int creatorId = GetCurrentUserId();

            var newRooms = new List<StreamingRoom>();

            if (request.Rooms != null && request.Rooms.Count > 0)
            {
                foreach (var rDto in request.Rooms)
                {
                    var id = rDto.Id.GetValueOrDefault();

                    if (id > 0 && existingById.TryGetValue(id, out var room))
                    {
                        // update room cũ...
                    }
                    else
                    {
                        // --- CREATE ROOM MỚI ---
                        var startAt = rDto.StartAt ?? e.StartAt;
                        var type = rDto.Type ?? StreamingRoomType.UPCOMING;

                        var newRoom = new StreamingRoom
                        {
                            RoomName = $"room-{Guid.NewGuid():N}",
                            Title = string.IsNullOrWhiteSpace(rDto.Title)
                                ? e.Title
                                : rDto.Title.Trim(),
                            CreatedBy = creatorId.ToString(),
                            StartAt = startAt,
                            Type = type,
                            IsActive = type == StreamingRoomType.LIVE,
                            ClosedAt = type == StreamingRoomType.CLOSED ? DateTime.UtcNow : null
                        };

                        e.StreamingRooms ??= new List<StreamingRoom>();
                        e.StreamingRooms.Add(newRoom);
                        newRooms.Add(newRoom);  // 👈 đang làm đúng
                    }
                }
            }

            // Lưu Event + Rooms (EF sẽ gán Id)
            await _eventRepo.UpdateAsync(e);

            // Tạo host participant + gửi email nhắc
            foreach (var room in newRooms)
            {
                await _participantRepo.AddAsync(new StreamingParticipant
                {
                    RoomId = room.Id,
                    UserId = creatorId,
                    Role = RoomRole.HOST,
                    Status = ParticipantStatus.WAITING,
                    RtcUid = creatorId.ToString()
                });

                // 👇 GỌI SENDGRID SCHEDULE
                await ScheduleRemindEmailsForRoomAsync(e, room);
            }

            var currentUserId = TryGetUserId();
            var resp = _mapper.Map<EventResponse>(e);
            resp.RegisteredByMe = currentUserId.HasValue &&
                e.Registrations.Any(r => r.UserId == currentUserId && !r.IsCancelled);

            return resp;

        }
        public async Task<PageResponse<EventResponse>> SearchEventsAsync(
        EventSearchRequest request)
        {
            // dùng includes để có Registrations + StreamingRooms cho AutoMapper
            var query = _eventRepo.GetEventsWithIncludes();

            // ----- 1. Filter keyword -----
            if (!string.IsNullOrWhiteSpace(request.Keyword))
            {
                var keyword = request.Keyword.Trim().ToLower();
                var unsignedKeyword = StringHelper.RemoveDiacritics(keyword);

                query = query.Where(e =>
                    e.Title.ToLower().Contains(keyword)
                // nếu có TitleUnsigned thì mở dòng này
                // || e.TitleUnsigned.Contains(unsignedKeyword)
                );
            }

            // ----- 2. Filter Category -----
            if (request.Category.HasValue)
            {
                query = query.Where(e => e.Category == request.Category.Value);
            }

            // ----- 3. Filter Tag ([Flags]) -----
            if (request.Tag.HasValue)
            {
                var tag = request.Tag.Value;
                query = query.Where(e => (e.Tags & tag) != 0);
            }

            // ----- 4. Filter Status (UPCOMING / LIVE / CLOSED / ...) -----
            if (request.Status.HasValue)
            {
                query = query.Where(e => e.Status == request.Status.Value);
            }

            // ----- 5. Filter theo khoảng StartAt -----
            if (request.FromDate.HasValue)
            {
                var fromUtc = request.FromDate.Value;
                if (fromUtc.Kind != DateTimeKind.Utc)
                    fromUtc = fromUtc.ToUniversalTime();

                query = query.Where(e => e.StartAt >= fromUtc);
            }

            if (request.ToDate.HasValue)
            {
                var toUtc = request.ToDate.Value;
                if (toUtc.Kind != DateTimeKind.Utc)
                    toUtc = toUtc.ToUniversalTime();

                query = query.Where(e => e.StartAt <= toUtc);
            }

            // ----- 6. Paging trên entity Event -----
            var pagedEntities = await query
                .OrderBy(e => e.StartAt)
                .ToPagedResponseAsync(request.Page, request.PageSize); // PageResponse<Event>

            var currentUserId = TryGetUserId();

            // ----- 7. Map từng Event -> EventResponse bằng AutoMapper -----
            var dtoItems = pagedEntities.Items.Select(e =>
            {
                var dto = _mapper.Map<EventResponse>(e);
                dto.RegisteredByMe = currentUserId.HasValue &&
                                     e.Registrations.Any(r => r.UserId == currentUserId && !r.IsCancelled);
                return dto;
            }).ToList();

            // ----- 8. Trả về PageResponse<EventResponse> đúng schema -----
            return new PageResponse<EventResponse>
            {
                Items = dtoItems,
                CurrentPages = pagedEntities.CurrentPages,
                PageSizes = pagedEntities.PageSizes,
                TotalPages = pagedEntities.TotalPages,
                TotalElements = pagedEntities.TotalElements
            };
        }

        public async Task<List<UserEventRegistrationResponse>> GetUserEventRegistrations()
        {
          
            int userId = GetCurrentUserId();

            var now = DateTime.UtcNow;
            var query = await _regRepo.GetEventRegistrationsQueryable()               
                .Where(g => g.UserId == userId && g.IsCancelled == false && g.Event.StartAt >= now)
                .OrderByDescending(g => g.CreatedAt)
                .Take(20)
                .ToListAsync();
          
            return _mapper.Map<List<UserEventRegistrationResponse>>(query);
        }
    }
}

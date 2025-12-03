using AutoMapper;
using Cultural_Heritage_System.Common;
using Cultural_Heritage_System.Dtos.Request.Event;
using Cultural_Heritage_System.Dtos.Response.Event;
using Cultural_Heritage_System.Middlewares;
using Cultural_Heritage_System.Models;
using Cultural_Heritage_System.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Cultural_Heritage_System.Services.Impl
{
    public class EventService : IEventService
    {
        private readonly IHttpContextAccessor _http;
        private readonly IEventRepository _eventRepo;
        private readonly IEventRegistrationRepository _regRepo;
        private readonly IStreamingParticipantRepository _participantRepo;
        private readonly IMapper _mapper;

        public EventService(
            IEventRepository eventRepo,
            IEventRegistrationRepository regRepo,
            IStreamingParticipantRepository participantRepo,
            IMapper mapper,
            IHttpContextAccessor http)
        {
            _eventRepo = eventRepo;
            _regRepo = regRepo;
            _participantRepo = participantRepo;
            _mapper = mapper;
            _http = http;
        }

        private int GetCurrentUserId()
        {
            var id = _http.HttpContext?.User.FindFirst("userId")?.Value;
            if (string.IsNullOrWhiteSpace(id))
                throw new AppException(ErrorCode.UNAUTHORIZED);
            return int.Parse(id);
        }

        private int? TryGetUserId()
        {
            var id = _http.HttpContext?.User.FindFirst("userId")?.Value;
            return string.IsNullOrWhiteSpace(id) ? (int?)null : int.Parse(id);
        }

        public async Task<EventResponse> CreateEventAsync(EventCreateRequest request)
        {
            var userId = GetCurrentUserId();

            var entity = _mapper.Map<Event>(request);
            entity.CreatedBy = userId.ToString(); ;

            var nowUtc = DateTime.UtcNow;
            var startUtc = request.StartAt.Kind == DateTimeKind.Utc
                ? request.StartAt
                : request.StartAt.ToUniversalTime();
            var closeUtc = request.CloseAt?.ToUniversalTime();

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

            await _eventRepo.AddAsync(entity);

            var created = await _eventRepo.GetEventByIdAsync(entity.Id)
                          ?? throw new AppException(ErrorCode.EVENT_NOT_FOUND);

            var resp = _mapper.Map<EventResponse>(created);
            resp.RegisteredByMe = false;
            return resp;
        }


        public async Task<EventResponse> UpdateEventAsync(EventUpdateRequest request)
        {
            var e = await _eventRepo.GetEventByIdAsync(request.Id)
                    ?? throw new AppException(ErrorCode.EVENT_NOT_FOUND);

            // TODO: check role admin ở đây nếu cần

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

            if (request.Status.HasValue)
                e.Status = request.Status.Value;

            if (request.Category.HasValue)
                e.Category = request.Category.Value;

            if (request.Tags.HasValue)
                e.Tags = request.Tags.Value;

            await _eventRepo.UpdateAsync(e);

            var currentUserId = TryGetUserId();
            var resp = _mapper.Map<EventResponse>(e);
            resp.RegisteredByMe = currentUserId.HasValue &&
                e.Registrations.Any(r => r.UserId == currentUserId && !r.IsCancelled);

            return resp;
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
                throw new AppException(ErrorCode.FORBIDDEN);

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
            int creatorId;
            if (!int.TryParse(e.CreatedBy ?? string.Empty, out creatorId))
            {
                creatorId = GetCurrentUserId();
            }

            var newRooms = new List<StreamingRoom>();

            if (request.Rooms != null && request.Rooms.Count > 0)
            {
                foreach (var rDto in request.Rooms)
                {
                    var id = rDto.Id.GetValueOrDefault();

                    if (id > 0 && existingById.TryGetValue(id, out var room))
                    {
                        // --- UPDATE ROOM CŨ ---
                        if (!string.IsNullOrWhiteSpace(rDto.Title))
                            room.Title = rDto.Title.Trim();

                        if (rDto.StartAt.HasValue)
                            room.StartAt = rDto.StartAt.Value;

                        if (rDto.Type.HasValue)
                        {
                            var newType = rDto.Type.Value;
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
                            CreatedBy = creatorId.ToString(),   // ✅ dùng string
                            StartAt = startAt,
                            Type = type,
                            IsActive = type == StreamingRoomType.LIVE,
                            ClosedAt = type == StreamingRoomType.CLOSED ? DateTime.UtcNow : null
                        };

                        e.StreamingRooms ??= new List<StreamingRoom>();
                        e.StreamingRooms.Add(newRoom);
                        newRooms.Add(newRoom);  // gom lại để tạo participant SAU khi save
                    }
                }
            }

            // Lưu Event + Rooms (EF sẽ gán Id cho newRooms)
            await _eventRepo.UpdateAsync(e);

            // Tạo host participant cho các room mới sau khi Id đã có
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
            }

            var currentUserId = TryGetUserId();
            var resp = _mapper.Map<EventResponse>(e);
            resp.RegisteredByMe = currentUserId.HasValue &&
                e.Registrations.Any(r => r.UserId == currentUserId && !r.IsCancelled);

            return resp;

        }
    }
}

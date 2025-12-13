using Cultural_Heritage_System.Common;
using Cultural_Heritage_System.Dtos.Request.Event;
using Cultural_Heritage_System.Dtos.Response;
using Cultural_Heritage_System.Dtos.Response.Event;
using Cultural_Heritage_System.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Cultural_Heritage_System.Controllers
{
    [Route("api/v1/events")]
    [ApiController]
    public class EventController : ControllerBase
    {
        private readonly IEventService _eventService;
        private readonly ILogger<EventController> _logger;

        public EventController(IEventService eventService, ILogger<EventController> logger)
        {
            _eventService = eventService;
            _logger = logger;
        }

        // ========== QUERY LIST / DETAIL ==========

        [HttpGet]
        public async Task<ApiResponse<List<EventResponse>>> GetEvents(
            [FromQuery] EventStatus? status,
            [FromQuery] DateTime? from)
        {
            var list = await _eventService.GetEventsAsync(status, from);
            return new ApiResponse<List<EventResponse>>(200, "Events retrieved successfully", list);
        }

        [HttpGet("{id:long}")]
        public async Task<ApiResponse<EventResponse>> GetEvent(long id)
        {
            var evt = await _eventService.GetEventDetailAsync(id);
            return new ApiResponse<EventResponse>(200, "Event detail", evt);
        }

        // ========== DELETE ==========

        [HttpDelete("{id:long}")]
        [Authorize(Roles = "ADMIN,STAFF")]       
        public async Task<ApiResponse<object>> DeleteEvent(long id)
        {
            await _eventService.DeleteEventAsync(id);
            return new ApiResponse<object>(200, "Event deleted");
        }

        // ========== REGISTRATION ==========

        [HttpPost("{id:long}/register")]
        [Authorize]
        public async Task<ApiResponse<EventRegistrationResponse>> Register(long id)
        {
            var res = await _eventService.RegisterAsync(id);
            return new ApiResponse<EventRegistrationResponse>(200, "Registered to event", res);
        }

        [HttpPost("{id:long}/unregister")]
        [Authorize]
        public async Task<ApiResponse<EventRegistrationResponse>> Unregister(long id)
        {
            var res = await _eventService.UnregisterAsync(id);
            return new ApiResponse<EventRegistrationResponse>(200, "Unregistered from event", res);
        }

        [HttpGet("{id:long}/registrations")]
        [Authorize(Roles = "ADMIN,STAFF")]      
        public async Task<ApiResponse<List<EventRegistrationUserResponse>>> GetEventRegistrations(long id)
        {
            try
            {
                var list = await _eventService.GetEventRegistrationsWithUserAsync(id);
                return new ApiResponse<List<EventRegistrationUserResponse>>(
                    200,
                    "Event registrations retrieved successfully",
                    list
                );
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in GetEventRegistrations");
                return new ApiResponse<List<EventRegistrationUserResponse>>(500, "Internal server error");
            }
        }

        // ========== WITH ROOMS (ADMIN) ==========

        [HttpPost("with-rooms")]
        [Authorize(Roles = "ADMIN,STAFF")]
        public async Task<ApiResponse<EventResponse>> CreateWithRooms(
            [FromBody] EventWithRoomsCreateRequest request)
        {
            var result = await _eventService.CreateEventWithRoomsAsync(request);
            return new ApiResponse<EventResponse>(
                code: 201,
                message: "Event with streaming rooms created successfully",
                result: result
            );
        }

        [HttpPut("with-rooms/{id:long}")]
        [Authorize(Roles = "ADMIN,STAFF")]
        public async Task<ApiResponse<EventResponse>> UpdateWithRooms(
            long id,
            [FromBody] EventWithRoomsUpdateRequest request)
        {
            request.Id = id;
            var result = await _eventService.UpdateEventWithRoomsAsync(request);
            return new ApiResponse<EventResponse>(
                code: 200,
                message: "Event with streaming rooms updated successfully",
                result: result
            );
        }

        // ========== SEARCH (PAGING + FILTER) ==========

        [HttpGet("search")]
        [Authorize(Roles = "ADMIN,STAFF")]
        public async Task<ApiResponse<PageResponse<EventResponse>>> SearchEvents(
            [FromQuery] EventSearchRequest request)
        {
            var result = await _eventService.SearchEventsAsync(request);

            return new ApiResponse<PageResponse<EventResponse>>
            {
                code = 200,
                result = result
            };
        }
    }
}

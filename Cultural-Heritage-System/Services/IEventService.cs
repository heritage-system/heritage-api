using Cultural_Heritage_System.Common;
using Cultural_Heritage_System.Dtos.Request.Event;
using Cultural_Heritage_System.Dtos.Response.Event;

namespace Cultural_Heritage_System.Services
{
    public interface IEventService
    {
        Task<EventResponse> CreateEventAsync(EventCreateRequest request);
        Task<EventResponse> UpdateEventAsync(EventUpdateRequest request);
        Task DeleteEventAsync(long id);
        Task<List<EventResponse>> GetEventsAsync(EventStatus? status, DateTime? from = null);
        Task<EventResponse> GetEventDetailAsync(long id);
        Task<EventRegistrationResponse> RegisterAsync(long eventId);
        Task<EventRegistrationResponse> UnregisterAsync(long eventId);
        Task<List<EventRegistrationUserResponse>> GetEventRegistrationsWithUserAsync(long eventId);
        Task<EventResponse> CreateEventWithRoomsAsync(EventWithRoomsCreateRequest request);
        Task<EventResponse> UpdateEventWithRoomsAsync(EventWithRoomsUpdateRequest request);

    }
}

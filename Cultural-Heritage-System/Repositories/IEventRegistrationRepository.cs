using Cultural_Heritage_System.Models;

namespace Cultural_Heritage_System.Repositories
{
    public interface IEventRegistrationRepository : IBaseRepository<EventRegistration>
    {
        Task<EventRegistration?> GetByEventAndUserAsync(long eventId, int userId);
        Task<List<EventRegistration>> GetByEventWithUserAsync(long eventId);
        IQueryable<EventRegistration> GetEventRegistrationsQueryable();

    }
}

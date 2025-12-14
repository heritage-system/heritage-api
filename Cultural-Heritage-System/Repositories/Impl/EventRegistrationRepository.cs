using Cultural_Heritage_System.DataAccessObjects;
using Cultural_Heritage_System.Models;
using Microsoft.Extensions.Logging;

namespace Cultural_Heritage_System.Repositories.Impl
{
    public class EventRegistrationRepository : BaseRepository<EventRegistration>, IEventRegistrationRepository
    {
        private readonly EventRegistrationDAO _dao;

        public EventRegistrationRepository(EventRegistrationDAO dao) : base(dao)
        {
            _dao = dao;
        }

        public async Task<EventRegistration?> GetByEventAndUserAsync(long eventId, int userId)
            => await _dao.GetByEventAndUserAsync(eventId, userId);

        public async Task<List<EventRegistration>> GetByEventWithUserAsync(long eventId)
         => await _dao.GetByEventWithUserAsync(eventId);

        public IQueryable<EventRegistration> GetEventRegistrationsQueryable()
         =>  _dao.GetEventRegistrationsQueryable();
    }

}

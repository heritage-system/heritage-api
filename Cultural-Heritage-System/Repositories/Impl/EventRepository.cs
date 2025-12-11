using Cultural_Heritage_System.DataAccessObjects;

namespace Cultural_Heritage_System.Repositories.Impl
{
    public class EventRepository : BaseRepository<Event>, IEventRepository
    {
        private readonly EventDAO _dao;

        public EventRepository(EventDAO dao) : base(dao)
        {
            _dao = dao;
        }



        public IQueryable<Event> GetEventsWithIncludes() => _dao.GetEventsWithIncludes();

        public Task<Event?> GetEventByIdAsync(long id) => _dao.GetEventByIdAsync(id);
    }
}

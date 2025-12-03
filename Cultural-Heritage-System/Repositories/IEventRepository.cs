using Cultural_Heritage_System.Models;

namespace Cultural_Heritage_System.Repositories
{
    public interface IEventRepository : IBaseRepository<Event>
    {
        IQueryable<Event> GetEventsQueryable();
        IQueryable<Event> GetEventsWithIncludes();
        Task<Event?> GetEventByIdAsync(long id);
    }
}

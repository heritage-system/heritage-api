namespace Cultural_Heritage_System.Repositories
{
    public interface IEventRepository : IBaseRepository<Event>
    {

        IQueryable<Event> GetEventsWithIncludes();
        Task<Event?> GetEventByIdAsync(long id);
    }
}

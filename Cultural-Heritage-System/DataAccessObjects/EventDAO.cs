using Cultural_Heritage_System.Models;
using Microsoft.EntityFrameworkCore;

namespace Cultural_Heritage_System.DataAccessObjects
{
    public class EventDAO : BaseDAO<Event>
    {
        private readonly ILogger<EventDAO> _logger;

        public EventDAO(AppDbContext context, ILogger<EventDAO> logger)
            : base(context)
        {
            _logger = logger;
        }

        public IQueryable<Event> GetEventsQueryable()
        {
            return _dbSet.AsQueryable();
        }

        public IQueryable<Event> GetEventsWithIncludes()
        {
            return _dbSet
                .Include(e => e.StreamingRooms)
                .Include(e => e.Registrations);
        }

        public Task<Event?> GetEventByIdAsync(long id)
        {
            return GetEventsWithIncludes().FirstOrDefaultAsync(e => e.Id == id);
        }
    }
}

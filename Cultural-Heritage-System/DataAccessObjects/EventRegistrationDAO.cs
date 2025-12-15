using Cultural_Heritage_System.Models;
using Microsoft.EntityFrameworkCore;

namespace Cultural_Heritage_System.DataAccessObjects
{
    public class EventRegistrationDAO : BaseDAO<EventRegistration>
    {
        public EventRegistrationDAO(AppDbContext ctx, ILogger<EventRegistrationDAO> logger)
            : base(ctx) { }

        public async Task<EventRegistration?> GetByEventAndUserAsync(long eventId, int userId)
        {
            return await _dbSet.FirstOrDefaultAsync(r => r.EventId == eventId && r.UserId == userId);
        }
        public async Task<List<EventRegistration>> GetByEventWithUserAsync(long eventId)
        {
            return await _context.EventRegistrations
                .Include(r => r.User)
                .Where(r => r.EventId == eventId && !r.IsCancelled)
                .OrderBy(r => r.RegisteredAt)
                .ToListAsync();
        }

        public IQueryable<EventRegistration> GetEventRegistrationsQueryable()
        {
            return _dbSet
                .Include(c => c.User)                  
                .Include(c => c.Event);
        }

    }
}

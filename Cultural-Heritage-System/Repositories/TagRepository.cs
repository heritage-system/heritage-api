using Cultural_Heritage_System.Models;
using medical_appointment_booking.Repositories;

namespace Cultural_Heritage_System.Repositories
{
    public class TagRepository : BaseRepository<Tag>
    {
        private readonly ILogger<TagRepository> _logger;

        public TagRepository(AppDbContext context, ILogger<TagRepository> logger)
            : base(context)
        {
            _logger = logger;
        }
        // Tạm thời 
        public IQueryable<Tag> GetTagsQueryable()
        {
            return _context.Tags.AsQueryable();
        }
    }
}

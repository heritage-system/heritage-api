using Cultural_Heritage_System.Models;
using Cultural_Heritage_System.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Cultural_Heritage_System.DataAccessObjects
{
    public class TagDAO : BaseDAO<Tag>
    {
        private readonly ILogger<TagDAO> _logger;

        public TagDAO(AppDbContext context, ILogger<TagDAO> logger)
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

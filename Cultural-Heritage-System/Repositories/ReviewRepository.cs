using Cultural_Heritage_System.Models;
using Microsoft.EntityFrameworkCore;

namespace Cultural_Heritage_System.Repositories
{
    public class ReviewRepository : BaseRepository<Review>
    {

        private readonly ILogger<ReviewRepository> _logger;

        public ReviewRepository(AppDbContext context, ILogger<ReviewRepository> logger)
            : base(context)
        {
            _logger = logger;
        }

        public IQueryable<Review> GetReviewsQueryable()
        {
            return _context.Reviews.AsQueryable();
        }
        public IQueryable<Review> GetReviewsWithIncludes()
        {
            return _dbSet
                .Include(r => r.User)
                .Include(r => r.Heritage)
                .Include(r => r.ReviewMedias)
                .Include(r => r.Likes).ThenInclude(l => l.User)
                .Include(r => r.Reports).ThenInclude(rep => rep.User)
                .Include(r => r.Replies).ThenInclude(reply => reply.User)
                .Include(r => r.Replies).ThenInclude(reply => reply.ReviewMedias)
                .Include(r => r.Replies).ThenInclude(reply => reply.Likes);
        }

    }
}

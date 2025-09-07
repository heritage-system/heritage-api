using Cultural_Heritage_System.Models;

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
    }
}

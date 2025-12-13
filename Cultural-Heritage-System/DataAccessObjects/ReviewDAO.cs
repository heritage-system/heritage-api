using Azure.Core;
using Cultural_Heritage_System.Models;
using Microsoft.EntityFrameworkCore;

namespace Cultural_Heritage_System.DataAccessObjects
{
    public class ReviewDAO : BaseDAO<Review>
    {

        private readonly ILogger<ReviewDAO> _logger;

        public ReviewDAO(AppDbContext context, ILogger<ReviewDAO> logger)
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
                .ThenInclude(u => u.Profile)
                .Include(r => r.Heritage)
                .Include(r => r.ReviewMedias)
                .Include(r => r.Likes).ThenInclude(l => l.User)
                .Include(r => r.Reports).ThenInclude(rep => rep.User)
                .Include(r => r.Replies).ThenInclude(reply => reply.User)
                .Include(r => r.Replies).ThenInclude(reply => reply.ReviewMedias)
                .Include(r => r.Replies).ThenInclude(reply => reply.Likes);
        }
        public async Task<List<Review>> GetReviewsHierarchy(long heritageId)
        {
            var reviews = await _dbSet
                .Include(r => r.User)
                .ThenInclude(u => u.Profile)
                .Include(r => r.ReviewMedias)
                .Include(r => r.Likes)
                .Where(r => r.HeritageId == heritageId && r.ParentReviewId == null)
                .ToListAsync();

            foreach (var review in reviews)
            {
                await LoadRepliesRecursive(review);
            }

            return reviews;
        }

        private async Task LoadRepliesRecursive(Review review)
        {
            await _dbSet.Entry(review)
                .Collection(r => r.Replies)
                .Query()
                .Include(r => r.User)
                .ThenInclude(u => u.Profile)
                .Include(r => r.ReviewMedias)
                .Include(r => r.Likes)
                .LoadAsync();

            foreach (var reply in review.Replies)
            {
                await LoadRepliesRecursive(reply);
            }
        }

        public async Task DeleteReviewWithRepliesAsync(long reviewId)
        {
            var review = await _context.Reviews
                .Include(r => r.Replies)
                .Include(r => r.ReviewMedias)
                .Include(r => r.Likes)
                .Include(r => r.Reports)
                .FirstOrDefaultAsync(r => r.Id == reviewId);

            if (review == null) return;

            foreach (var reply in review.Replies.ToList())
            {
                await DeleteReviewWithRepliesAsync(reply.Id);
            }

            _context.ReviewMedias.RemoveRange(review.ReviewMedias ?? []);
            _context.ReviewLikes.RemoveRange(review.Likes ?? []);
            _context.ReviewReports.RemoveRange(review.Reports ?? []);
            _context.Reviews.Remove(review);
            await _context.SaveChangesAsync();
        }

        public async Task<Review?> GetReviewById(long id)
        {
            return await _dbSet
                .Include(r => r.User)
                .ThenInclude(u => u.Profile)
                .Include(r => r.Heritage)
                .Include(r => r.ReviewMedias)
                .Include(r => r.Likes).ThenInclude(l => l.User)
                .Include(r => r.Reports).ThenInclude(rep => rep.User)
                .Include(r => r.Replies).ThenInclude(reply => reply.User)
                .Include(r => r.Replies).ThenInclude(reply => reply.ReviewMedias)
                .Include(r => r.Replies).ThenInclude(reply => reply.Likes)
                .FirstOrDefaultAsync(r => r.Id == id);
        }

    }
}

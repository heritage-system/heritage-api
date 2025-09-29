using Cultural_Heritage_System.Models;
using Microsoft.EntityFrameworkCore;

namespace Cultural_Heritage_System.Repositories
{
    public class ContributionReviewRepository : BaseRepository<ContributionReview>
    {
                private readonly ILogger<ContributionReviewRepository> _logger;

        public ContributionReviewRepository(AppDbContext context, ILogger<ContributionReviewRepository> logger)
            : base(context)
        {
            _logger = logger;
        }
   
        public Task<ContributionReview> GetContributionReviewById(long reviewId)
        {
            return _dbSet
                .Include(r => r.User)
                .ThenInclude(u => u.Profile)
                .Include(r => r.Contribution)               
                .Include(r => r.Likes).ThenInclude(l => l.User)             
                .Include(r => r.Replies).ThenInclude(reply => reply.User)              
                .Include(r => r.Replies).ThenInclude(reply => reply.Likes)
                 .FirstAsync(r => r.Id == reviewId);
        }
        public async Task<List<ContributionReview>> GetContributionReviewReviewsHierarchy(long contributionId)
        {
            var reviews = await _dbSet
                .Include(r => r.User)       
                .ThenInclude(u => u.Profile)
                .Include(r => r.Likes)
                .Where(r => r.ContributionId == contributionId && r.ParentReviewId == null)
                .OrderByDescending(r => r.CreatedAt)
                .ToListAsync();

            foreach (var review in reviews)
            {
                await LoadRepliesRecursive(review);
            }

            return reviews;
        }

        private async Task LoadRepliesRecursive(ContributionReview review)
        {
            await _dbSet.Entry(review)
                .Collection(r => r.Replies)
                .Query()
                .Include(r => r.User)               
                .Include(r => r.Likes)
                .LoadAsync();

            foreach (var reply in review.Replies)
            {
                await LoadRepliesRecursive(reply);
            }
        }

        public async Task DeleteContributionReviewWithRepliesAsync(long reviewId)
        {
            var review = await _dbSet
                .Include(r => r.Replies)               
                .Include(r => r.Likes)             
                .FirstOrDefaultAsync(r => r.Id == reviewId);

            if (review == null) return;

            foreach (var reply in review.Replies.ToList())
            {
                await DeleteContributionReviewWithRepliesAsync(reply.Id);
            }
          
            _context.ContributionReviewLike.RemoveRange(review.Likes ?? []);       
            _dbSet.Remove(review);
            await _context.SaveChangesAsync();
        }


    }
}

using Cultural_Heritage_System.Models;
using Microsoft.EntityFrameworkCore;

namespace Cultural_Heritage_System.Repositories
{
    public interface IContributionReviewRepository : IBaseRepository<ContributionReview>
    {
        Task<ContributionReview> GetContributionReviewById(long reviewId);
        Task<List<ContributionReview>> GetContributionReviewReviewsHierarchy(long contributionId);     
        Task DeleteContributionReviewWithRepliesAsync(long reviewId);      
    }
}

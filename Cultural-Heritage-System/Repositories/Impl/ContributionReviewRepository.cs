using Cultural_Heritage_System.DataAccessObjects;
using Cultural_Heritage_System.Models;
using Microsoft.EntityFrameworkCore;

namespace Cultural_Heritage_System.Repositories.Impl
{
    public class ContributionReviewRepository : BaseRepository<ContributionReview>, IContributionReviewRepository
    {
        private readonly ContributionReviewDAO _entityDAO;

        public ContributionReviewRepository(ContributionReviewDAO dao) : base(dao)
        {
            _entityDAO = dao;
        }
        public Task<ContributionReview> GetContributionReviewById(long reviewId)
        {
            return _entityDAO.GetContributionReviewById(reviewId);
        }
        public async Task<List<ContributionReview>> GetContributionReviewReviewsHierarchy(long contributionId)
        {           
            return await _entityDAO.GetContributionReviewReviewsHierarchy(contributionId);
        }
  
        public async Task DeleteContributionReviewWithRepliesAsync(long reviewId)
        {          
            await _entityDAO.DeleteContributionReviewWithRepliesAsync(reviewId);
        }
      
    }
}

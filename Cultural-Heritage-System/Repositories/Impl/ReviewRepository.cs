using Azure.Core;
using Cultural_Heritage_System.DataAccessObjects;
using Cultural_Heritage_System.Models;
using Microsoft.EntityFrameworkCore;

namespace Cultural_Heritage_System.Repositories.Impl
{
    public class ReviewRepository : BaseRepository<Review>, IReviewRepository
    {

        private readonly ReviewDAO _entityDAO;

        public ReviewRepository(ReviewDAO dao) : base(dao)
        {
            _entityDAO = dao;
        }

        public IQueryable<Review> GetReviewsQueryable()
        {
            return _entityDAO.GetReviewsQueryable();
        }
        public IQueryable<Review> GetReviewsWithIncludes()
        {
            return _entityDAO.GetReviewsWithIncludes();
        }
        public async Task<List<Review>> GetReviewsHierarchy(long heritageId)
        {
           
            return await _entityDAO.GetReviewsHierarchy(heritageId);
        }
  
        public async Task DeleteReviewWithRepliesAsync(long reviewId)
        {           
            await _entityDAO.DeleteReviewWithRepliesAsync(reviewId);
        }

        public async Task<Review?> GetReviewById(long id)
        {
            return await _entityDAO.GetReviewById(id);
        }

    }
}

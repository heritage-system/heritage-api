using Azure.Core;
using Cultural_Heritage_System.Models;
using Microsoft.EntityFrameworkCore;

namespace Cultural_Heritage_System.Repositories
{
    public interface IReviewRepository : IBaseRepository<Review>
    {

        IQueryable<Review> GetReviewsQueryable();
        IQueryable<Review> GetReviewsWithIncludes();
        Task<List<Review>> GetReviewsHierarchy(long heritageId);
        Task DeleteReviewWithRepliesAsync(long reviewId);
        Task<Review?> GetReviewById(long id);   
    }
}

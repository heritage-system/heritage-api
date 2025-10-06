using Cultural_Heritage_System.Common;
using Cultural_Heritage_System.Dtos.Models;
using Cultural_Heritage_System.Models;
using Cultural_Heritage_System.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Cultural_Heritage_System.Repositories
{
    public interface IContributionRepository : IBaseRepository<Contribution>
    {
        IQueryable<Contribution> GetContributionsQueryable();
        IQueryable<Contribution> GetApprovedContributionsQueryable();
        Task<Contribution?> GetContributionById(long id);
        Task<Contribution?> GetContributionByIdAndStatus(long id, ContributionStatus contributionStatus);
        Task<List<TrendingContributorDto>> GetTopContributorsAsync();
        IQueryable<Contribution> GetContributionsByContributorIdQueryable(int contributorId);      
    }
}
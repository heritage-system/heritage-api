using Cultural_Heritage_System.Common;
using Cultural_Heritage_System.DataAccessObjects;
using Cultural_Heritage_System.Dtos.Models;
using Cultural_Heritage_System.Models;
using Cultural_Heritage_System.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Cultural_Heritage_System.Repositories.Impl
{
    public class ContributionRepository : BaseRepository<Contribution>, IContributionRepository
    {
        private readonly ContributionDAO _entityDAO;

        public ContributionRepository(ContributionDAO dao) : base(dao)
        {
            _entityDAO = dao;
        }

        public IQueryable<Contribution> GetContributionsQueryable()
        {
            return _entityDAO.GetContributionsQueryable();
        }

        public IQueryable<Contribution> GetApprovedContributionsQueryable()
        {
            return _entityDAO.GetApprovedContributionsQueryable();
        }

        public async Task<Contribution?> GetContributionById(long id)
        {
            return await _entityDAO.GetContributionById(id);
        }

        public async Task<Contribution?> GetContributionByIdAndStatus(long id,ContributionStatus contributionStatus)
        {
            return await _entityDAO.GetContributionByIdAndStatus(id, contributionStatus);
        }

        public async Task<List<TrendingContributorDto>> GetTopContributorsAsync()
        {          

            return await _entityDAO.GetTopContributorsAsync();
        }

        public IQueryable<Contribution> GetContributionsByContributorIdQueryable(int contributorId)
        {
            return _entityDAO.GetContributionsByContributorIdQueryable(contributorId);
        }

        public IQueryable<Contribution> GetContributionsByStaffIdQueryable(int staffId)
        {
            return _entityDAO.GetContributionsByStaffIdQueryable(staffId);
        }

        public Task<Contribution?> GetContributionForStaffAsync(long contributionId, int staffId)
        {
            return _entityDAO.GetContributionForStaffAsync(contributionId, staffId);
        }
             
    }
}
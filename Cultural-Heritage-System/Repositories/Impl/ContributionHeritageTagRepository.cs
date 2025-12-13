using Cultural_Heritage_System.DataAccessObjects;
using Cultural_Heritage_System.Models;
using Cultural_Heritage_System.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Cultural_Heritage_System.Repositories.Impl
{
    public class ContributionHeritageTagRepository : BaseRepository<ContributionHeritageTag>, IContributionHeritageTagRepository
    {
        private readonly ContributionHeritageTagDAO _entityDAO;

        public ContributionHeritageTagRepository(ContributionHeritageTagDAO dao) : base(dao)
        {
            _entityDAO = dao;
        }

        public IQueryable<ContributionHeritageTag> GetContributionHeritageTagsQueryable()
        {
            return _entityDAO.GetContributionHeritageTagsQueryable();
        }
     
    }
}
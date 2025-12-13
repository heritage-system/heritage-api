using Cultural_Heritage_System.Models;
using Cultural_Heritage_System.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Cultural_Heritage_System.DataAccessObjects
{
    public class ContributionHeritageTagDAO : BaseDAO<ContributionHeritageTag>
    {
        private readonly ILogger<ContributionHeritageTagDAO> _logger;

        public ContributionHeritageTagDAO(AppDbContext context, ILogger<ContributionHeritageTagDAO> logger)
            : base(context)
        {
            _logger = logger;
        }

        public IQueryable<ContributionHeritageTag> GetContributionHeritageTagsQueryable()
        {
            return _dbSet
                .Include(h => h.Heritage)     
                .Include(h => h.Contribution)
                .AsQueryable();
        }
     
    }
}
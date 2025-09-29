using Cultural_Heritage_System.Models;
using Cultural_Heritage_System.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Cultural_Heritage_System.Repositories
{
    public class ContributionHeritageTagRepository : BaseRepository<ContributionHeritageTag>
    {
        private readonly ILogger<ContributionHeritageTagRepository> _logger;

        public ContributionHeritageTagRepository(AppDbContext context, ILogger<ContributionHeritageTagRepository> logger)
            : base(context)
        {
            _logger = logger;
        }

        public IQueryable<ContributionHeritageTag> GetContributionHeritageTagsQueryable()
        {
            return _dbSet
                .Include(h => h.Heritage)                    
                .AsQueryable();
        }
     
    }
}
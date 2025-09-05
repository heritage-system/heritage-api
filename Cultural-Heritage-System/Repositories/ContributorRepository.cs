using Cultural_Heritage_System.Models;
using Cultural_Heritage_System.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Cultural_Heritage_System.Repositories
{
    public class ContributorRepository : BaseRepository<Contributor>
    {
        private readonly ILogger<ContributorRepository> _logger;

        public ContributorRepository(AppDbContext context, ILogger<ContributorRepository> logger)
            : base(context)
        {
            _logger = logger;
        }

        public async Task<Contributor?> GetContributorById(int id)
        {
            return await _context.Contributors
                .Include(c => c.User)                 
                .Include(c => c.Contributions)        
                .FirstOrDefaultAsync(c => c.Id == id);
        }

        public IQueryable<Contributor> GetContributorsQueryable()
        {
            return _context.Contributors
                .Include(c => c.User)                 
                .Include(c => c.Contributions);      
        }

        public async Task<Contributor?> GetContributorByUserId(int id)
        {
            return await _context.Contributors
                .Include(c => c.User)
                .Include(c => c.Contributions)
                .FirstOrDefaultAsync(c => c.UserId == id);
        }
    }
}
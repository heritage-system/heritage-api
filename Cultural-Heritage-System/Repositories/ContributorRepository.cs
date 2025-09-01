using Cultural_Heritage_System.Models;
using medical_appointment_booking.Repositories;
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

        public IQueryable<Contributor> GetContributorsQueryable()
        {
            return _context.Contributors
                .Include(c => c.User)
                .AsQueryable();
        }

        public async Task<Contributor?> GetContributorById(int id)
        {
            return await _dbSet
                .Include(c => c.User)
                .FirstOrDefaultAsync(c => c.Id == id);
        }

        //public async Task<List<Contributor>> SearchContributors(string keyword)
        //{
        //    return await _dbSet
        //        .Include(c => c.User)
        //        .Where(c =>
        //            (c.User.FullName != null && c.User.FullName.Contains(keyword)) ||
        //            (c.Expertise != null && c.Expertise.Contains(keyword)) ||
        //            (c.Bio != null && c.Bio.Contains(keyword))
        //        )
        //        .OrderBy(c => c.Id)
        //        .ToListAsync();
        //}
    }
}
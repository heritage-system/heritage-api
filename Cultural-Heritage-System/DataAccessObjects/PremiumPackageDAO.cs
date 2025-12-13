using Cultural_Heritage_System.Models;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace Cultural_Heritage_System.DataAccessObjects
{
    public class PremiumPackageDAO : BaseDAO<PremiumPackage>
    {
        public PremiumPackageDAO(AppDbContext dbContext) : base(dbContext)
        {
        }

        public override async Task<IEnumerable<PremiumPackage>> GetAllAsync()
        {
            return await _dbSet
                .Include(p => p.PackageBenefits)
                    .ThenInclude(pb => pb.Benefit)
                .ToListAsync();
        }

        public async Task<PremiumPackage?> GetByIdAsync(int packageId)
        {
            return await _dbSet
                .Include(p => p.PackageBenefits)
                    .ThenInclude(pb => pb.Benefit)
                .FirstOrDefaultAsync(p => p.Id == packageId);
        }

        public IQueryable<PremiumPackage> GetQueryable()
        {
            return _dbSet
                .Include(p => p.PackageBenefits)
                    .ThenInclude(pb => pb.Benefit)
                .AsQueryable();
        }

        public async Task<IEnumerable<PremiumPackage>> GetActivePackageAsync()
        {
            return await _dbSet
                .Where(p => p.IsActive)
                .Include(p => p.PackageBenefits) 
                .ThenInclude(pb => pb.Benefit)  
                .ToListAsync();                  
        }
    }

}

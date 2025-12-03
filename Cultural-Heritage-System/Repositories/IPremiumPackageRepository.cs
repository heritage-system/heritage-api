using Cultural_Heritage_System.Models;

namespace Cultural_Heritage_System.Repositories
{
    public interface IPremiumPackageRepository : IBaseRepository<PremiumPackage>
    {
        IQueryable<PremiumPackage> GetQueryable();
        Task<IEnumerable<PremiumPackage>> GetAllAsync();
        Task<PremiumPackage?> GetByIdAsync(int id);
        Task<IEnumerable<PremiumPackage>> GetActivePackageAsyns();
    }
}

using Cultural_Heritage_System.DataAccessObjects;
using Cultural_Heritage_System.Models;

namespace Cultural_Heritage_System.Repositories.Impl
{
    public class PremiumPackageRepository : BaseRepository<PremiumPackage>, IPremiumPackageRepository
    {
        private readonly PremiumPackageDAO _dao;

        public PremiumPackageRepository(PremiumPackageDAO dao) : base(dao)
        {
            _dao = dao;
        }

        public async Task<IEnumerable<PremiumPackage>> GetAllAsync()
        {
            return await _dao.GetAllAsync();
        }

        public async Task<PremiumPackage?> GetByIdAsync(int id)
        {
            return await _dao.GetByIdAsync(id);
        }

        public IQueryable<PremiumPackage> GetQueryable()
        {
            return _dao.GetQueryable();
        }

        public async Task<IEnumerable<PremiumPackage>> GetActivePackageAsyns()
        {
            return await _dao.GetActivePackageAsync();
        }
    }
}

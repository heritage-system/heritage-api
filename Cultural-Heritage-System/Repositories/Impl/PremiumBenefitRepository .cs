using Cultural_Heritage_System.DataAccessObjects;
using Cultural_Heritage_System.Models;

namespace Cultural_Heritage_System.Repositories.Impl
{
    public class PremiumBenefitRepository : BaseRepository<PremiumBenefit>, IPremiumBenefitRepository
    {
        private readonly PremiumBenefitDAO _entityDAO;

        public PremiumBenefitRepository(PremiumBenefitDAO dao) : base(dao)
        {
            _entityDAO = dao;
        }

        public IQueryable<PremiumBenefit> GetQueryable() => _entityDAO.GetQueryable();
    }
}

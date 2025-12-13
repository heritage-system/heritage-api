using Cultural_Heritage_System.Models;

namespace Cultural_Heritage_System.Repositories
{
    public interface IPremiumBenefitRepository : IBaseRepository<PremiumBenefit>
    {
        IQueryable<PremiumBenefit> GetQueryable();
    }
}

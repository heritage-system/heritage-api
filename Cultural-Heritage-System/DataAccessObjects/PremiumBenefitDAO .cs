using Cultural_Heritage_System.Models;
using Microsoft.EntityFrameworkCore;

namespace Cultural_Heritage_System.DataAccessObjects
{
    public class PremiumBenefitDAO : BaseDAO<PremiumBenefit>
    {

        public PremiumBenefitDAO(AppDbContext dbContext) : base(dbContext) { }

        public IQueryable<PremiumBenefit> GetQueryable()
        {
            return _dbSet.AsQueryable();
        }
    }

}

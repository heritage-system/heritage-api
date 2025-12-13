using Cultural_Heritage_System.Models;
using Cultural_Heritage_System.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Cultural_Heritage_System.Repositories
{
    public interface IContributionHeritageTagRepository : IBaseRepository<ContributionHeritageTag>
    {
        IQueryable<ContributionHeritageTag> GetContributionHeritageTagsQueryable();       
    }
}
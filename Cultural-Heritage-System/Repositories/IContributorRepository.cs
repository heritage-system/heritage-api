using Cultural_Heritage_System.Models;
using Cultural_Heritage_System.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Cultural_Heritage_System.Repositories
{
    public interface IContributorRepository : IBaseRepository<Contributor>
    {
        Task<Contributor?> GetContributorById(int id);
        IQueryable<Contributor> GetContributorsQueryable();
        Task<Contributor?> GetContributorByUserId(int id);
       
    }
}
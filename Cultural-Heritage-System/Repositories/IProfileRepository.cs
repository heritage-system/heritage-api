using Cultural_Heritage_System.Models;
using Cultural_Heritage_System.Repositories;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Cultural_Heritage_System.Repositories
{
    public interface IProfileRepository : IBaseRepository<Profile>
    {

        Task<Profile?> GetProfileByUserIdAsync(long userId);
        IQueryable<Profile> GetQueryable();
        
    }
}

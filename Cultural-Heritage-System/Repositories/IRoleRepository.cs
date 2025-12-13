using Cultural_Heritage_System.Middlewares;
using Cultural_Heritage_System.Models;
using Microsoft.EntityFrameworkCore;

namespace Cultural_Heritage_System.Repositories
{
    public interface IRoleRepository : IBaseRepository<Role>
    {
        Task<Role> CreateRole(Role role);
        Task<Role?> FindByRoleName(string RoleName);
        
    }
}

using Cultural_Heritage_System.DataAccessObjects;
using Cultural_Heritage_System.Middlewares;
using Cultural_Heritage_System.Models;
using Microsoft.EntityFrameworkCore;

namespace Cultural_Heritage_System.Repositories.Impl
{
    public class RoleRepository : BaseRepository<Role>, IRoleRepository
    {

        private readonly RoleDAO _entityDAO;

        public RoleRepository(RoleDAO dao) : base(dao)
        {
            _entityDAO = dao;
        }

    public async Task<Role> CreateRole(Role role)
        {
           
            return await _entityDAO.CreateRole(role);
        }

        public async Task<Role?> FindByRoleName(string RoleName)
        {
            return await _entityDAO.FindByRoleName(RoleName);

        }   

    }
}

using Cultural_Heritage_System.Middlewares;
using Cultural_Heritage_System.Models;
using Microsoft.EntityFrameworkCore;

namespace Cultural_Heritage_System.Repositories
{
    public class RoleRepository
    {
        private readonly AppDbContext _context;

        public RoleRepository(AppDbContext context)
        {
            this._context = context;
        }

        public async Task<Role> CreateRole(Role role)
        {
            var existingRole = await _context.Roles
                .FirstOrDefaultAsync(x => x.Name == role.Name);

            if (existingRole != null)
            {
                throw new AppException(ErrorCode.ROLE_EXISTED);
            }

            _context.Roles.Add(role);
            await _context.SaveChangesAsync();

            return role;
        }

        public async Task<Role?> FindByRoleName(string RoleName)
        {
            return await _context.Roles
                .FirstOrDefaultAsync(x => x.Name == RoleName);

        }

    }
}

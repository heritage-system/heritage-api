using Cultural_Heritage_System.Middlewares;
using Cultural_Heritage_System.Models;
using Microsoft.EntityFrameworkCore;

namespace Cultural_Heritage_System.DataAccessObjects
{
    public class RoleDAO : BaseDAO<Role>
    {

        private readonly ILogger<RoleDAO> _logger;

        public RoleDAO(AppDbContext context, ILogger<RoleDAO> logger)
            : base(context)
        {
            _logger = logger;
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

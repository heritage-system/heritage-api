using CloudinaryDotNet.Core;
using Cultural_Heritage_System.Models;
using Cultural_Heritage_System.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Cultural_Heritage_System.DataAccessObjects
{
    public class PasswordResetDAO : BaseDAO<PasswordReset>
    {
        private readonly ILogger<PasswordResetDAO> _logger;

        public PasswordResetDAO(AppDbContext context, ILogger<PasswordResetDAO> logger)
            : base(context)
        {
            _logger = logger;
        }
        public async Task<PasswordReset?> GetLatestByUserIdAsync(int userId)
        {
            return await _context.PasswordResets
                .Where(r => r.UserId == userId)
                .OrderByDescending(r => r.CreatedAt)
                .FirstOrDefaultAsync();
        }
    }
}

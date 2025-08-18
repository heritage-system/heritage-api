using CloudinaryDotNet.Core;
using Cultural_Heritage_System.Models;
using medical_appointment_booking.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Cultural_Heritage_System.Repositories
{
    public class PasswordResetRepository : BaseRepository<PasswordReset>
    {
        private readonly ILogger<PasswordResetRepository> _logger;

        public PasswordResetRepository(AppDbContext context, ILogger<PasswordResetRepository> logger)
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

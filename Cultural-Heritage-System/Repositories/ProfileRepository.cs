using Cultural_Heritage_System.Models;
using medical_appointment_booking.Repositories;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Cultural_Heritage_System.Repositories
{
    public class ProfileRepository : BaseRepository<Profile>
    {
        public ProfileRepository(AppDbContext dbContext) : base(dbContext)
        {
        }

        public async Task<Profile?> GetProfileByUserIdAsync(long userId)
        {
            return await _dbSet.FirstOrDefaultAsync(p => p.UserId == userId);
        }
      
    }
}

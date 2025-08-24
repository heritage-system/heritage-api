using Cultural_Heritage_System.Models;
using medical_appointment_booking.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Cultural_Heritage_System.Repositories
{
    public class HeritageLocationRepository : BaseRepository<Heritage>
    {
        public HeritageLocationRepository(AppDbContext context) : base(context) { }

        public async Task<List<Heritage>> GetAllWithCoordinatesAndLocations()
        {
            return await _dbSet
                .Include(h => h.Coordinates)
                .Include(h => h.HeritageLocations)
                    .ThenInclude(hl => hl.Location)
                .ToListAsync();
        }
    }
}

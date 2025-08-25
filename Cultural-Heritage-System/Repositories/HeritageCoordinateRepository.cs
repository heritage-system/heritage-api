using Cultural_Heritage_System.Models;
using medical_appointment_booking.Repositories;

namespace Cultural_Heritage_System.Repositories
{
    public class HeritageCoordinateRepository : BaseRepository<HeritageCoordinate>
    {
        private readonly ILogger<HeritageCoordinateRepository> _logger;

        public HeritageCoordinateRepository(AppDbContext context, ILogger<HeritageCoordinateRepository> logger)
            : base(context)
        {
            _logger = logger;
        }
    }
}

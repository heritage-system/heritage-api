using Cultural_Heritage_System.Dtos.Response;
using Cultural_Heritage_System.Models;
using medical_appointment_booking.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Cultural_Heritage_System.Repositories
{
    public class HeritageLocationRepository : BaseRepository<HeritageLocation>
    {
        public HeritageLocationRepository(AppDbContext context) : base(context) { }

       
    }
}

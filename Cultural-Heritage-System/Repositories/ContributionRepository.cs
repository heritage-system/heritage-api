using Cultural_Heritage_System.Models;
using medical_appointment_booking.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Cultural_Heritage_System.Repositories
{
    public class ContributionRepository : BaseRepository<Contribution>
    {
        private readonly ILogger<ContributionRepository> _logger;

        public ContributionRepository(AppDbContext context, ILogger<ContributionRepository> logger)
            : base(context)
        {
            _logger = logger;
        }       
    }
}
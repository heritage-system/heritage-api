using Cultural_Heritage_System.Common;
using Cultural_Heritage_System.Dtos.Models;
using Cultural_Heritage_System.Models;
using Cultural_Heritage_System.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Cultural_Heritage_System.Repositories
{
    public interface IPanoramaInteractionPointRepository : IBaseRepository<PanoramaInteractionPoint>
    { 
        Task<PanoramaInteractionPoint?> GetPanoramaInteractionPointById(long id);
    }
}
using Cultural_Heritage_System.Models;
using Cultural_Heritage_System.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Cultural_Heritage_System.Repositories
{
    public interface IPanoramaSceneUnlockRepository : IBaseRepository<PanoramaSceneUnlock>
    {
        Task<PanoramaSceneUnlock> GetPanoramaSceneUnlockByUserAndPanoramaScene(int userId, long sceneId);
        IQueryable<PanoramaSceneUnlock> GetPanoramaSceneUnlocksQueryByUserId(int userId);
        Task<bool> IsPanoramaSceneUnlockExists(int userId, long sceneId);
        Task<int> GetPanoramaSceneUnlockCountByUserId(int userId);
        
    }
}

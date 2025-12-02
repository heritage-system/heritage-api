using Cultural_Heritage_System.DataAccessObjects;
using Cultural_Heritage_System.Models;
using Cultural_Heritage_System.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Cultural_Heritage_System.Repositories.Impl
{
    public class PanoramaSceneUnlockRepository : BaseRepository<PanoramaSceneUnlock>, IPanoramaSceneUnlockRepository
    {
        private readonly PanoramaSceneUnlockDAO _entityDAO;

        public PanoramaSceneUnlockRepository(PanoramaSceneUnlockDAO dao) : base(dao)
        {
            _entityDAO = dao;
        }

        public async Task<PanoramaSceneUnlock> GetPanoramaSceneUnlockByUserAndPanoramaScene(int userId, long sceneId)
        {
            return await _entityDAO.GetPanoramaSceneUnlockByUserAndPanoramaScene(userId, sceneId);
        }

 

        public IQueryable<PanoramaSceneUnlock> GetPanoramaSceneUnlocksQueryByUserId(int userId)
        {
            return _entityDAO.GetPanoramaSceneUnlocksQueryByUserId(userId);
        }

        public async Task<bool> IsPanoramaSceneUnlockExists(int userId, long sceneId)
        {
            return await _entityDAO.IsPanoramaSceneUnlockExists(userId, sceneId);
        }

        public async Task<int> GetPanoramaSceneUnlockCountByUserId(int userId)
        {
            return await _entityDAO.GetPanoramaSceneUnlockCountByUserId(userId);
        }
    }
}

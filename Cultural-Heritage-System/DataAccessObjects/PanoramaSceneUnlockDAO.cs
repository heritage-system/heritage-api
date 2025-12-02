using Cultural_Heritage_System.Models;
using Cultural_Heritage_System.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Cultural_Heritage_System.DataAccessObjects
{
    public class PanoramaSceneUnlockDAO : BaseDAO<PanoramaSceneUnlock>
    {
        private readonly ILogger<PanoramaSceneUnlockDAO> _logger;

        public PanoramaSceneUnlockDAO(AppDbContext context, ILogger<PanoramaSceneUnlockDAO> logger)
            : base(context)
        {
            _logger = logger;
        }

        public async Task<PanoramaSceneUnlock> GetPanoramaSceneUnlockByUserAndPanoramaScene(int userId, long sceneId)
        {
            return await _dbSet
                .FirstOrDefaultAsync(f => f.UserId == userId && f.PanoramaSceneId == sceneId);
        }

        public IQueryable<PanoramaSceneUnlock> GetPanoramaSceneUnlocksQueryByUserId(int userId)
        {
            return _dbSet
                .Include(f => f.PanoramaScene)            
                .Where(f => f.UserId == userId)
                .OrderByDescending(f => f.CreatedAt);
        }

        public async Task<bool> IsPanoramaSceneUnlockExists(int userId, long sceneId)
        {
            return await _dbSet
                .AnyAsync(f => f.UserId == userId && f.PanoramaSceneId == sceneId);
        }

        public async Task<int> GetPanoramaSceneUnlockCountByUserId(int userId)
        {
            return await _dbSet
                .CountAsync(f => f.UserId == userId);
        }
    }
}

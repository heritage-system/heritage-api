using Cultural_Heritage_System.DataAccessObjects;
using Cultural_Heritage_System.Models;
using Cultural_Heritage_System.Repositories;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Cultural_Heritage_System.Repositories.Impl
{
    public class ProfileRepository : BaseRepository<Profile>, IProfileRepository
    {
        private readonly ProfileDAO _entityDAO;

        public ProfileRepository(ProfileDAO dao) : base(dao)
        {
            _entityDAO = dao;
        }

        public async Task<Profile?> GetProfileByUserIdAsync(long userId)
        {
            return await _entityDAO.GetProfileByUserIdAsync(userId);
        }

        public IQueryable<Profile> GetQueryable()
        {
            return _entityDAO.GetQueryable();
        }
    }
}

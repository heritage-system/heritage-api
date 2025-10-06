using Cultural_Heritage_System.DataAccessObjects;
using Cultural_Heritage_System.Models;
using Cultural_Heritage_System.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Cultural_Heritage_System.Repositories.Impl
{
    public class RefreshTokenRepository : BaseRepository<RefreshToken>, IRefreshTokenRepository
    {
        private readonly RefreshTokenDAO _entityDAO;

        public RefreshTokenRepository(RefreshTokenDAO dao) : base(dao)
        {
            _entityDAO = dao;
        }

        public async Task<RefreshToken?> FindByTokenAsync(string requestToken)
        {
            return await _entityDAO.FindByTokenAsync(requestToken);
        }
    }
}
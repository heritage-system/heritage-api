using Cultural_Heritage_System.DataAccessObjects;
using Cultural_Heritage_System.Models;
using Cultural_Heritage_System.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Cultural_Heritage_System.Repositories.Impl
{
    public class ConfirmTokenRepository : BaseRepository<ConfirmToken>, IConfirmTokenRepository
    {
        private readonly ConfirmTokenDAO _entityDAO;

        public ConfirmTokenRepository(ConfirmTokenDAO dao) : base(dao)
        {
            _entityDAO = dao;
        }

        public async Task<ConfirmToken?> FindByTokenAsync(int userId,string requestToken)
        {
            return await _entityDAO.FindByTokenAsync(userId, requestToken);
        }
        public async Task<ConfirmToken?> FindTokenByUserIdAsync(int userId)
        {
            return await _entityDAO.FindTokenByUserIdAsync(userId);
        }

    }
}
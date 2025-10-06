using CloudinaryDotNet.Core;
using Cultural_Heritage_System.DataAccessObjects;
using Cultural_Heritage_System.Models;
using Cultural_Heritage_System.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Cultural_Heritage_System.Repositories.Impl
{
    public class PasswordResetRepository : BaseRepository<PasswordReset>, IPasswordResetRepository
    {
        private readonly PasswordResetDAO _entityDAO;

        public PasswordResetRepository(PasswordResetDAO dao) : base(dao)
        {
            _entityDAO = dao;
        }
        public async Task<PasswordReset?> GetLatestByUserIdAsync(int userId)
        {
            return await _entityDAO.GetLatestByUserIdAsync(userId);
        }
    }
}

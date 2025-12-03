using Cultural_Heritage_System.Models;
using Cultural_Heritage_System.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Cultural_Heritage_System.Repositories
{
    public interface IConfirmTokenRepository : IBaseRepository<ConfirmToken>
    {
        Task<ConfirmToken?> FindByTokenAsync(int userId, string requestToken);
        Task<ConfirmToken?> FindTokenByUserIdAsync(int userId);


    }
}
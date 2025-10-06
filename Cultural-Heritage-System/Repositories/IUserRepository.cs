using Cultural_Heritage_System.Common;
using Cultural_Heritage_System.Models;
using Cultural_Heritage_System.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Cultural_Heritage_System.Repositories
{
    public interface IUserRepository : IBaseRepository<User>
    {

        Task<User?> FindUserByEmailOrUserName(string emailOrUserName);
        Task<User?> FindUserByEmail(string email);
        Task<List<User>> GetAllUsers(int page, int size);
        Task<int> GetTotalUsersCount();
        Task<User?> FindUserById(long id);
        IQueryable<User> GetQueryable();
    }
}

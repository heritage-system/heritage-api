using Cultural_Heritage_System.Common;
using Cultural_Heritage_System.DataAccessObjects;
using Cultural_Heritage_System.Models;
using Cultural_Heritage_System.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Cultural_Heritage_System.Repositories.Impl
{
    public class UserRepository : BaseRepository<User>, IUserRepository
    {
        private readonly UserDAO _entityDAO;

        public UserRepository(UserDAO dao) : base(dao)
        {
            _entityDAO = dao;
        }

        public async Task<User?> FindUserByEmailOrUserName(string emailOrUserName)
        {
            return await _entityDAO.FindUserByEmailOrUserName(emailOrUserName);
        }

        public async Task<User?> FindUserByEmail(string email)
        {
            return await _entityDAO.FindUserByEmail(email);
        }

        public async Task<List<User>> GetAllUsers(int page, int size)
        {
            return await _entityDAO.GetAllUsers(page, size);
        }


        public async Task<int> GetTotalUsersCount()
        {
            return await _entityDAO.GetTotalUsersCount();
        }

        public async Task<User?> FindUserById(long id)
        {
            return await _entityDAO.FindUserById(id);
        }

        public IQueryable<User> GetQueryable()
        {
            return _entityDAO.GetQueryable();
        }

    }
}

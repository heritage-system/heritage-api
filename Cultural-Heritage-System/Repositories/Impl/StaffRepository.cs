using Cultural_Heritage_System.Common;
using Cultural_Heritage_System.DataAccessObjects;
using Cultural_Heritage_System.Models;
using Cultural_Heritage_System.Repositories;
using Microsoft.EntityFrameworkCore;
using SendGrid.Helpers.Mail;

namespace Cultural_Heritage_System.Repositories.Impl
{
    public class StaffRepository : BaseRepository<Staff>, IStaffRepository
    {
        private readonly StaffDAO _entityDAO;

        public StaffRepository(StaffDAO dao) : base(dao)
        {
            _entityDAO = dao;
        }
        public async Task<Staff?> GetStaffById(int id)
        {
            return await _entityDAO.GetStaffById(id);
        }

        public IQueryable<Staff> GetStaffsQueryable()
        {
            return _entityDAO.GetStaffsQueryable();
        }

        public async Task<Staff?> GetStaffByUserId(int id)
        {
            return await _entityDAO.GetStaffByUserId(id);
        }

        public async Task<List<Staff>> GetActiveReviewersAsync()
        {
            return await _entityDAO.GetActiveReviewersAsync();
        }

        // Lấy id staff gần nhất đã được assign
        public async Task<int?> GetLastAssignedStaffIdAsync()
        {
            return await _entityDAO.GetLastAssignedStaffIdAsync();
        }

        public async Task<Staff?> GetByUserIdAsync(int userId)
        {
            return  await _entityDAO.GetByUserIdAsync(userId);
        }

    }
}
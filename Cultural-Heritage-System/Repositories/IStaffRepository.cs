using Cultural_Heritage_System.Common;
using Cultural_Heritage_System.Models;
using Cultural_Heritage_System.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Cultural_Heritage_System.Repositories
{
    public interface IStaffRepository : IBaseRepository<Staff>
    {
        Task<Staff?> GetStaffById(int id);
        IQueryable<Staff> GetStaffsQueryable();
        Task<Staff?> GetStaffByUserId(int id);
        Task<List<Staff>> GetActiveReviewersAsync();
        Task<int?> GetLastAssignedStaffIdAsync();
        Task<Staff?> GetByUserIdAsync(int userId);
    }
}
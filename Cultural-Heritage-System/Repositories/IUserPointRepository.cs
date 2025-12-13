using Cultural_Heritage_System.Models;
using Cultural_Heritage_System.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Cultural_Heritage_System.Repositories
{
    public interface IUserPointRepository : IBaseRepository<UserPoint>
    {
        Task<UserPoint?> GetUserPointById(int id);
        IQueryable<UserPoint> GetUserPointsQueryable();
        Task<UserPoint?> GetUserPointByUserId(int id);
       
    }
}
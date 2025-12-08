
using Cultural_Heritage_System.Dtos.Request.Heritage;
using Cultural_Heritage_System.Dtos.Request.UserPoint;
using Cultural_Heritage_System.Dtos.Response;
using Cultural_Heritage_System.Dtos.Response.UserPoint;
using Cultural_Heritage_System.Models;


namespace Cultural_Heritage_System.Services
{
    public interface IUserPointService
    {     
        Task<bool> UpdateUserPoint(UserPointUpdateRequest request);
        Task<UserPointResponse> GetUserPointByUserId();
    }
}

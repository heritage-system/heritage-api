using Cultural_Heritage_System.Dtos.Response.Contributor;

namespace Cultural_Heritage_System.Services
{
    public interface IUserCacheService
    {
        Task<List<DropdownUserResponse>> GetCachedUsersAsync();
        Task SetCachedUsersAsync(List<DropdownUserResponse> users);
    }
}

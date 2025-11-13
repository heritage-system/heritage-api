using Cultural_Heritage_System.Common;
using Cultural_Heritage_System.Dtos.Request;
using Cultural_Heritage_System.Dtos.Request.User;
using Cultural_Heritage_System.Dtos.Response;
using Cultural_Heritage_System.Dtos.Response.User;

namespace Cultural_Heritage_System.Services
{
    public interface IUserService
    {
        Task<UserCreationResponse> CreateUser(UserCreationRequest request);
        Task ChangePassword(ChangePasswordRequest request);
        Task<UpdateProfileResponse> UpdateProfile(UpdateProfileRequest request);
        Task<UpdateProfileResponse> GetProfile();
        Task<PageResponse<UserSearchResponse>> SearchMemberForAdmin(UserSearchRequest request);
        Task<UserCreationResponse> CreateUserForAdmin(UserCreationByAdminRequest request);
        Task<UserDetailResponse> GetUserDetailForAdmin(int id);
        Task<bool> ChangeUserStatusForAdmin(int id, UserStatus status);

    }
}

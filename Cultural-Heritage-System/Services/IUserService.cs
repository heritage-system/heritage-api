using Cultural_Heritage_System.Dtos.Request;
using Cultural_Heritage_System.Dtos.Response;

namespace Cultural_Heritage_System.Services
{
    public interface IUserService
    {
        Task<UserCreationResponse> CreateUser(UserCreationRequest request);
        Task ChangePassword(ChangePasswordRequest request);
        Task<UpdateProfileResponse> UpdateProfile(UpdateProfileRequest request);
        Task<UpdateProfileResponse> GetProfile();

    }
}

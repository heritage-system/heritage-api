using Cultural_Heritage_System.Dtos.Request;
using Cultural_Heritage_System.Dtos.Response;

namespace Cultural_Heritage_System.Services
{
    public interface IAuthService
    {
        Task<SignInResponse> SignIn(SignInRequest request);
        Task<SignInResponse> RefreshToken(string refreshToken);
        Task<SignInResponse> SignInWithGoogle(string code);

        Task<SignInResponse> SignInWithFacebook(string code);
        Task<bool> ConfirmEmail(int userId, string token);
        Task SignOut();
    }
}

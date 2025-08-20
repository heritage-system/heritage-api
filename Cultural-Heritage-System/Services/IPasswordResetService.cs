namespace Cultural_Heritage_System.Services
{
    public interface IPasswordResetService
    {
        Task<ForgotPasswordResponse> SendResetCodeAsync(string email, string ipAddress);
        Task<bool> ResetPasswordAsync(string email, string otp, string newPassword);
    }
}

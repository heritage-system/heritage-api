namespace Cultural_Heritage_System.Services
{
    public interface IMailService
    {
        Task SendEmailWelcome(string to, string user, DateTime registrationDate);
        Task SendEmailOtpResetPassword(string to,string otp, DateTime expiresAt);

    }
}


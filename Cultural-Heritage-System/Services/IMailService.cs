namespace Cultural_Heritage_System.Services
{
    public interface IMailService
    {
        Task SendEmailWelcome(string to, string user, DateTime registrationDate);
        Task SendEmailOtpResetPassword(string to,string otp, DateTime expiresAt);
        Task SendEmailAnswerReport(string to, long reportId, string answer);
        Task SendEmailWelcomeForAdmin(string to, string user, string password, string role, string username);
    }
}


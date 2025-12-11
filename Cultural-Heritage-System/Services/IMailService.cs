namespace Cultural_Heritage_System.Services
{
    public interface IMailService
    {
        Task SendEmailWelcome(string to, string user, DateTime registrationDate, string activationLink);
        Task SendEmailOtpResetPassword(string to,string otp, DateTime expiresAt);
        Task SendEmailAnswerReport(string to, string userName, string heritageName, string reportTime, string reportContent, string replyMessage);
        Task SendEmailWelcomeForAdmin(string to, string user, string password, string role, string username);
        Task SendRemindEmail(string to, string userName, string eventName, string startTime, string eventDate, string joinUrl, DateTime scheduleTimeUtc);
    }
}


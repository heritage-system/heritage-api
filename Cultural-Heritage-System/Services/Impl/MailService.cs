using SendGrid;
using SendGrid.Helpers.Mail;
namespace Cultural_Heritage_System.Services.Impl
{
    public class MailService : IMailService
    {
        private readonly string _sendGridApiKey;
        private readonly string _welcomeTemplateId;
        private readonly string _otpTemplateId;
        private readonly string _emailFrom;
        private readonly ILogger<MailService> _logger;

        public MailService(IConfiguration configuration, ILogger<MailService> logger)
        {
            _sendGridApiKey = configuration["SendGrid:ApiKey"]
                ?? throw new ArgumentNullException("SendGrid:ApiKey is required");
            _welcomeTemplateId = configuration["SendGrid:WelcomeTemplateId"]
                ?? throw new ArgumentNullException("SendGrid:WelcomeTemplateId is required");
            _otpTemplateId = configuration["SendGrid:OtpTemplateId"]
               ?? throw new ArgumentNullException("SendGrid:OtpTemplateId is required");
            _emailFrom = configuration["SendGrid:FromEmail"]
                ?? throw new ArgumentNullException("SendGrid:FromEmail is required");

            _logger = logger;
        }

        public async Task SendEmailWelcome(string to, string user, DateTime registrationDate)
        {
            var client = new SendGridClient(_sendGridApiKey);
            var from = new EmailAddress(_emailFrom, "VTFP");
            var toEmail = new EmailAddress(to);
            var msg = new SendGridMessage
            {
                TemplateId = _welcomeTemplateId,
                From = from,            
            };
            msg.AddTo(toEmail);
            msg.SetTemplateData(new
            {
                email = to,            
                user = user
                //registration_date = registrationDate.ToString("dd/MM/yyyy")
            });

            var response = await client.SendEmailAsync(msg);
            if (response.StatusCode == System.Net.HttpStatusCode.Accepted)
            {
                _logger.LogInformation("Email sent successfully to {Email}", to);
            }
            else
            {
                _logger.LogError("Failed to send email to {Email}. Status: {StatusCode}", to, response.StatusCode);
            }
        }

        public async Task SendEmailOtpResetPassword(string to,string otp, DateTime expiresAt)
        {
            var client = new SendGridClient(_sendGridApiKey);
            var from = new EmailAddress(_emailFrom, "VTFP");
            var toEmail = new EmailAddress(to);
            var msg = new SendGridMessage
            {
                TemplateId = _otpTemplateId,
                From = from,
            };
            msg.AddTo(toEmail);
            msg.SetTemplateData(new
            {
                otp = otp,
                expiresAt = expiresAt
                //registration_date = registrationDate.ToString("dd/MM/yyyy")
            });

            var response = await client.SendEmailAsync(msg);
            if (response.StatusCode == System.Net.HttpStatusCode.Accepted)
            {
                _logger.LogInformation("Email sent successfully to {Email}", to);
            }
            else
            {
                _logger.LogError("Failed to send email to {Email}. Status: {StatusCode}", to, response.StatusCode);
            }
        }
    }

}
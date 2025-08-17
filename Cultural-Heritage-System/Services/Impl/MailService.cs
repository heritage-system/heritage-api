//using SendGrid;
//using SendGrid.Helpers.Mail;
//namespace Cultural_Heritage_System.Services.Impl
//{
//    public class MailService : IMailService
//    {
//        private readonly string _sendGridApiKey;
//        private readonly string _welcomeTemplateId;
//        private readonly string _doctorWelcomeTemplateId;
//        private readonly string _emailFrom;
//        private readonly ILogger<MailService> _logger;

//        public MailService(IConfiguration configuration, ILogger<MailService> logger)
//        {
//            _sendGridApiKey = configuration["SendGrid:ApiKey"]
//                ?? throw new ArgumentNullException("SendGrid:ApiKey is required");
//            _welcomeTemplateId = configuration["SendGrid:WelcomeTemplateId"]
//                ?? throw new ArgumentNullException("SendGrid:WelcomeTemplateId is required");
//            _doctorWelcomeTemplateId = configuration["SendGrid:DoctorWelcomeTemplateId"]
//                ?? throw new ArgumentNullException("SendGrid:DoctorWelcomeTemplateId is required");
//            _emailFrom = configuration["SendGrid:FromEmail"]
//                ?? throw new ArgumentNullException("SendGrid:FromEmail is required");
//            _logger = logger;
//        }

//        public async Task SendEmailWelcome(string to, string name, string phone, DateTime registrationDate)
//        {
//            var client = new SendGridClient(_sendGridApiKey);
//            var from = new EmailAddress(_emailFrom, "Medical Appointment");
//            var toEmail = new EmailAddress(to);
//            var msg = new SendGridMessage
//            {
//                TemplateId = _welcomeTemplateId,
//                From = from,
//                Subject = "Welcome to Medical Appointment"
//            };
//            msg.AddTo(toEmail);
//            msg.SetTemplateData(new
//            {
//                email = to,
//                phone = phone,
//                full_name = name,
//                registration_date = registrationDate.ToString("dd/MM/yyyy")
//            });

//            var response = await client.SendEmailAsync(msg);
//            if (response.StatusCode == System.Net.HttpStatusCode.Accepted)
//            {
//                _logger.LogInformation("Email sent successfully to {Email}", to);
//            }
//            else
//            {
//                _logger.LogError("Failed to send email to {Email}. Status: {StatusCode}", to, response.StatusCode);
//            }
//        }

//        public async Task SendDoctorWelcomeEmail(string to, string doctorName, string email, string temporaryPassword,
//            string licenseNumber, string specialty, decimal consultationFee, DateTime registrationDate)
//        {
//            var client = new SendGridClient(_sendGridApiKey);
//            var from = new EmailAddress(_emailFrom, "Medical Appointment System");
//            var toEmail = new EmailAddress(to);

//            var msg = new SendGridMessage
//            {
//                TemplateId = _doctorWelcomeTemplateId,
//                From = from,
//                Subject = "Welcome to Medical Appointment - Doctor Account Created"
//            };

//            msg.AddTo(toEmail);
//            msg.SetTemplateData(new
//            {
//                doctor_name = doctorName,
//                email = email,
//                temporary_password = temporaryPassword,
//                license_number = licenseNumber,
//                specialty = specialty,
//                consultation_fee = consultationFee.ToString("C"),
//                registration_date = registrationDate.ToString("dd/MM/yyyy HH:mm"),
//                login_url = "https://your-medical-app.com/doctor/login", 
//                support_email = _emailFrom,
//                current_year = DateTime.Now.Year
//            });

//            var response = await client.SendEmailAsync(msg);
//            if (response.StatusCode == System.Net.HttpStatusCode.Accepted)
//            {
//                _logger.LogInformation("Doctor welcome email sent successfully to {Email}", to);
//            }
//            else
//            {
//                _logger.LogError("Failed to send doctor welcome email to {Email}. Status: {StatusCode}", to, response.StatusCode);
//            }
//        }

//        public async Task SendReceptionistWelcomeEmail(string to, string email, string temporaryPassword,DateTime registrationDate)
//        {
//            var client = new SendGridClient(_sendGridApiKey);
//            var from = new EmailAddress(_emailFrom, "Medical Appointment System");
//            var toEmail = new EmailAddress(to);

//            var msg = new SendGridMessage
//            {
//                TemplateId = _doctorWelcomeTemplateId,
//                From = from,
//                Subject = "Welcome to Medical Appointment - Receptionist Account Created"
//            };

//            msg.AddTo(toEmail);
//            msg.SetTemplateData(new
//            {           
//                email = email,
//                temporary_password = temporaryPassword,             
//                registration_date = registrationDate.ToString("dd/MM/yyyy HH:mm"),
//                login_url = "https://your-medical-app.com/doctor/login",
//                support_email = _emailFrom,
//                current_year = DateTime.Now.Year
//            });

//            var response = await client.SendEmailAsync(msg);
//            if (response.StatusCode == System.Net.HttpStatusCode.Accepted)
//            {
//                _logger.LogInformation("Receptionist welcome email sent successfully to {Email}", to);
//            }
//            else
//            {
//                _logger.LogError("Failed to send receptionist welcome email to {Email}. Status: {StatusCode}", to, response.StatusCode);
//            }
//        }
//    }

//}
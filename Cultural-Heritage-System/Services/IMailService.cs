namespace Cultural_Heritage_System.Services
{
    public interface IMailService
    {
        Task SendEmailWelcome(string to, string name, string phone, DateTime registrationDate);
        Task SendDoctorWelcomeEmail(string to, string doctorName, string email, string temporaryPassword,
            string licenseNumber, string specialty, decimal consultationFee, DateTime registrationDate);

        Task SendReceptionistWelcomeEmail(string to, string email, string temporaryPassword, DateTime registrationDate);
    }
}


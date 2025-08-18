
using AutoMapper;
using Cultural_Heritage_System.Dtos.Response;
using Cultural_Heritage_System.Middlewares;
using Cultural_Heritage_System.Models;
using Cultural_Heritage_System.Repositories;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using System.Security.Cryptography;
using System.Text;

namespace Cultural_Heritage_System.Services.Impl
{
    public class PasswordResetService : IPasswordResetService
    {
        private readonly UserRepository userRepository;
        private readonly PasswordResetRepository resetRepository;
        private readonly PasswordHasher<User> passwordHasher;
        private readonly IMailService mailService;
        private readonly IMapper mapper;
        private readonly string pepper = "SECRET_PEPPER"; 

        public PasswordResetService(
            UserRepository userRepository,
            PasswordResetRepository resetRepository,         
            IMailService mailService, IMapper mapper)
        {
            this.userRepository = userRepository;
            this.resetRepository = resetRepository;
            this.passwordHasher = new PasswordHasher<User>();
            this.mailService = mailService;
            this.mapper = mapper;
        }

        private string HashOtp(string otp)
        {
            using var hmac = new HMACSHA256(Encoding.UTF8.GetBytes(pepper));
            var hash = hmac.ComputeHash(Encoding.UTF8.GetBytes(otp));
            return Convert.ToBase64String(hash);
        }

        public async Task<ForgotPasswordResponse> SendResetCodeAsync(string email, string ipAddress)
        {
            var user = await userRepository.FindUserByEmail(email);
            if (user == null)
            {               
                throw new AppException(ErrorCode.USER_NOT_EXISTED);
            }


            var rnd = new Random();
            var otp = rnd.Next(100000, 999999).ToString();

          
            var otpHash = HashOtp(otp);

           
            var reset = new PasswordReset
            {
                UserId = user.Id,
                CodeHash = otpHash,
                ExpiresAt = DateTime.UtcNow.AddMinutes(10),
                CreatedIp = ipAddress,
                Attempts = 0,
                Used = false
            };
            await resetRepository.AddAsync(reset);

           
            await mailService.SendEmailOtpResetPassword(email,otp, reset.ExpiresAt);

            return mapper.Map<ForgotPasswordResponse>(reset);
        }

        public async Task<bool> ResetPasswordAsync(string email, string otp, string newPassword)
        {
            var user = await userRepository.FindUserByEmail(email);
            if (user == null)
            {
                throw new AppException(ErrorCode.USER_NOT_EXISTED);              
            }

            var reset = await resetRepository.GetLatestByUserIdAsync(user.Id);
            if (reset == null || reset.Used || reset.ExpiresAt < DateTime.UtcNow)
            {
                throw new AppException(ErrorCode.EXPIRED_OTP);
            }

            if (reset.Attempts >= 5)
            {
                throw new AppException(ErrorCode.ATTEMPTS_OVER_LIMIT);
            }
           
            var hashInput = HashOtp(otp);

            if (hashInput != reset.CodeHash)
            {
                reset.Attempts++;
                await resetRepository.UpdateAsync(reset);
                throw new AppException(ErrorCode.OTP_NOT_TRUE);
            }
       
            user.PasswordHash = passwordHasher.HashPassword(user, newPassword);
            await userRepository.UpdateAsync(user);

            reset.Used = true;
            reset.VerifiedAt = DateTime.UtcNow;
            await resetRepository.UpdateAsync(reset);

            return true;
        }
    }
}
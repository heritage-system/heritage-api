using AutoMapper;
using Cultural_Heritage_System.Common;
using Cultural_Heritage_System.Dtos.Request;
using Cultural_Heritage_System.Dtos.Response;
using Cultural_Heritage_System.Middlewares;
using Cultural_Heritage_System.Models;
using Cultural_Heritage_System.Repositories;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;

namespace Cultural_Heritage_System.Services.Impl
{
    public class UserService : IUserService
    {
        private readonly IHttpContextAccessor httpContextAccessor;
        private readonly UserRepository userRepository;
        private readonly ProfileRepository profileRepository;
        private readonly RoleRepository roleRepository;
        private readonly PasswordHasher<User> passwordHasher;
        private readonly IMapper mapper;
        private readonly IMailService mailService;
        private readonly ILogger<UserService> logger;

        public UserService(UserRepository userRepository, RoleRepository roleRepository, ILogger<UserService> logger, IMailService mailService,
            ProfileRepository profileRepository, IMapper mapper, IHttpContextAccessor httpContextAccessor)
        {
            this.userRepository = userRepository;
            this.roleRepository = roleRepository;
            this.passwordHasher = new PasswordHasher<User>();
            this.logger = logger;
            this.profileRepository = profileRepository;
            this.mailService = mailService;
            this.mapper = mapper;
            this.httpContextAccessor = httpContextAccessor;
        }

        public async Task<UserCreationResponse> CreateUser(UserCreationRequest request)
        {
            var userOptinal = await userRepository.FindUserByEmail(request.Email);
            if (userOptinal != null)
            {
                logger.LogError("User Existed");
                throw new AppException(ErrorCode.USER_EXISTED);
            }

            User user = mapper.Map<User>(request);          
            user.PasswordHash = passwordHasher.HashPassword(user, request.Password.Trim());
             
            var role = await roleRepository.FindByRoleName(DefinitionRole.ADMIN);
            if (role == null)
            {
                role = new Role();
                role.Name = DefinitionRole.ADMIN;
                await roleRepository.CreateRole(role);
            }
            user.RoleId = role.Id;
            await userRepository.AddAsync(user);

            var profile = new Models.Profile
            {
                UserId = user.Id,                            
            };

            await profileRepository.AddAsync(profile);

          

            await mailService.SendEmailWelcome(user.Email, user.UserName, user.CreatedAt);

            return mapper.Map<UserCreationResponse>(user); 
        }

        public async Task ChangePassword(ChangePasswordRequest request)
        {
            var accountIdClaim = httpContextAccessor.HttpContext?.User.FindFirst("userId")?.Value;
            if (string.IsNullOrEmpty(accountIdClaim))
            {
                throw new AppException(ErrorCode.UNAUTHORIZED);
            }

            var accountId = int.Parse(accountIdClaim);
            var existingUser = await userRepository.FindUserById(accountId);
            if (existingUser == null)
            {
                throw new AppException(ErrorCode.USER_NOT_EXISTED);
            }

            var passwordVerificationResult = passwordHasher.VerifyHashedPassword(existingUser, existingUser.PasswordHash, request.OldPassword);
            if (passwordVerificationResult == PasswordVerificationResult.Failed)
            {
                throw new AppException(ErrorCode.UNAUTHORIZED);
            }

            existingUser.PasswordHash = passwordHasher.HashPassword(existingUser, request.NewPassword);
            await userRepository.UpdateAsync(existingUser);
        }
    }
}

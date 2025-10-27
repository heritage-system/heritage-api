using AutoMapper;
using Cultural_Heritage_System.Common;
using Cultural_Heritage_System.Dtos.Request;
using Cultural_Heritage_System.Dtos.Response;
using Cultural_Heritage_System.Middlewares;
using Cultural_Heritage_System.Models;
using Cultural_Heritage_System.Repositories;
using Microsoft.AspNetCore.Identity;

namespace Cultural_Heritage_System.Services.Impl
{
    public class UserService : IUserService
    {
        private readonly IHttpContextAccessor httpContextAccessor;
        private readonly IUserRepository userRepository;
        private readonly IProfileRepository profileRepository;
        private readonly IRoleRepository roleRepository;
        private readonly PasswordHasher<User> passwordHasher;
        private readonly IMapper mapper;
        private readonly IMailService mailService;
        private readonly ILogger<UserService> logger;
        private readonly ISubscriptionRepository subscriptionRepository;
        private readonly IContributorRepository contributorRepository;
        public UserService(IUserRepository userRepository, IRoleRepository roleRepository, ILogger<UserService> logger, IMailService mailService,
            IProfileRepository profileRepository, IMapper mapper, IHttpContextAccessor httpContextAccessor, ISubscriptionRepository subscriptionRepository, IContributorRepository contributorRepository)
        {
            this.userRepository = userRepository;
            this.roleRepository = roleRepository;
            this.passwordHasher = new PasswordHasher<User>();
            this.logger = logger;
            this.profileRepository = profileRepository;
            this.mailService = mailService;
            this.mapper = mapper;
            this.httpContextAccessor = httpContextAccessor;
            this.subscriptionRepository = subscriptionRepository;
            this.contributorRepository = contributorRepository;
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

            var role = await roleRepository.FindByRoleName(DefinitionRole.STAFF);
            if (role == null)
            {
                role = new Role();
                role.Name = DefinitionRole.STAFF;
                await roleRepository.CreateRole(role);
            }
            user.RoleId = role.Id;
            await userRepository.AddAsync(user);

            var profile = new Models.Profile
            {
                UserId = user.Id,
                FullName = request.FullName,
                AvatarUrl = "https://res.cloudinary.com/dea92gqx4/image/upload/v1761033759/Windows_10_Default_Profile_Picture.svg_x71ugm.png"
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

        public async Task<UpdateProfileResponse> UpdateProfile(UpdateProfileRequest request)
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
            var existingProfile = await profileRepository.GetProfileByUserIdAsync(accountId);
            if (existingProfile == null)
            {
                existingProfile = new Cultural_Heritage_System.Models.Profile { UserId = accountId };
                await profileRepository.AddAsync(existingProfile);
            }

            mapper.Map(request, existingUser);
            //existingUser.GenerateUnsignedFields();
            await userRepository.UpdateAsync(existingUser);

            mapper.Map(request, existingProfile);
            //existingProfile.GenerateUnsignedFields();
            await profileRepository.UpdateAsync(existingProfile);

            var response = mapper.Map<UpdateProfileResponse>(existingUser);
            mapper.Map(existingProfile, response);

            var activeSub = await subscriptionRepository.GetActiveSubscription(accountId);
            response.isPremium = activeSub?.Status == SubscriptionStatus.ACTIVE;

            var currentContributor = await contributorRepository.GetContributorByUserId(accountId);
            response.isContributor = currentContributor?.Status == ContributorStatus.ACTIVE;
            return response;
        }

        public async Task<UpdateProfileResponse> GetProfile()
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

            var existingProfile = await profileRepository.GetProfileByUserIdAsync(accountId);

            var response = mapper.Map<UpdateProfileResponse>(existingUser);
            if (existingProfile != null)
            {
                mapper.Map(existingProfile, response);
            }
            var activeSub = await subscriptionRepository.GetActiveSubscription(accountId);
            response.isPremium = activeSub?.Status == SubscriptionStatus.ACTIVE;

            var currentContributor = await contributorRepository.GetContributorByUserId(accountId);
            response.isContributor = currentContributor?.Status == ContributorStatus.ACTIVE;
            return response;
        }
    }
}

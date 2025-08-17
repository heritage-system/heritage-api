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
        private readonly UserRepository userRepository;
        private readonly ProfileRepository profileRepository;
        private readonly RoleRepository roleRepository;
        private readonly PasswordHasher<User> passwordHasher;
        private readonly IMapper mapper;
        //private readonly IMailService mailService;
        private readonly ILogger<UserService> logger;

        public UserService(UserRepository userRepository, RoleRepository roleRepository, ILogger<UserService> logger,
            ProfileRepository profileRepository, IMapper mapper)
        {
            this.userRepository = userRepository;
            this.roleRepository = roleRepository;
            this.passwordHasher = new PasswordHasher<User>();
            this.logger = logger;
            this.profileRepository = profileRepository;
            //this.mailService = mailService;
            this.mapper = mapper;
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

          

            //await mailService.SendEmailWelcome(user.Email, profile.FirstName + " " + profile.LastName, user.Phone, user.CreatedAt);

            return mapper.Map<UserCreationResponse>(user); 
        }
    }
}

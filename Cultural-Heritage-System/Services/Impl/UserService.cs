using AutoMapper;
using AutoMapper.QueryableExtensions;
using Cultural_Heritage_System.Common;
using Cultural_Heritage_System.Dtos.Request;
using Cultural_Heritage_System.Dtos.Request.User;
using Cultural_Heritage_System.Dtos.Response;
using Cultural_Heritage_System.Dtos.Response.User;
using Cultural_Heritage_System.Helpers;
using Cultural_Heritage_System.Middlewares;
using Cultural_Heritage_System.Models;
using Cultural_Heritage_System.Repositories;
using Cultural_Heritage_System.Repositories.Impl;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

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
        private readonly IStaffRepository staffRepository;
        private readonly IConfirmTokenRepository confirmTokenRepository;
        private readonly IUserPointRepository userPointRepository;

        private readonly string _baseUrl;
        public UserService(IConfiguration configuration,IUserRepository userRepository, IRoleRepository roleRepository, ILogger<UserService> logger, IMailService mailService,
            IProfileRepository profileRepository, IMapper mapper, IHttpContextAccessor httpContextAccessor, ISubscriptionRepository subscriptionRepository, IContributorRepository contributorRepository, IStaffRepository staffRepository, IConfirmTokenRepository confirmTokenRepository, IUserPointRepository userPointRepository)
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
            this.staffRepository = staffRepository;
            this.confirmTokenRepository = confirmTokenRepository;

            _baseUrl = configuration["BaseUrl:FEUrl"]
                ?? throw new ArgumentNullException("BaseUrl:FEUrl is required");
            this.userPointRepository = userPointRepository;
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
            user.UserStatus = UserStatus.PENDING_VERIFICATION;

            var role = await roleRepository.FindByRoleName(DefinitionRole.MEMBER);
            if (role == null)
            {
                role = new Role();
                role.Name = DefinitionRole.MEMBER;
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

            var userPoint = new UserPoint
            {
                UserId = user.Id,
            };

            await userPointRepository.AddAsync(userPoint);

            var confirmToken = Guid.NewGuid().ToString("N");
            await confirmTokenRepository.AddAsync(new ConfirmToken
            {
                UserId = user.Id,
                Token = confirmToken,
            });

            string confirmLink = $"{_baseUrl}/confirm-email-address?uid={user.Id}&token={confirmToken}";

            await mailService.SendEmailWelcome(user.Email, user.UserName, user.CreatedAt,confirmLink);

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

        public async Task<PageResponse<UserSearchResponse>> SearchMemberForAdmin(UserSearchRequest request)
        {
            var query = userRepository.GetQueryable();

            // ---- Filter ----
            if (!string.IsNullOrWhiteSpace(request.Keyword))
            {
                var searchTerm = request.Keyword.Trim().ToLower();
                var unsignedTerm = StringHelper.RemoveDiacritics(searchTerm);

                query = query.Where(x =>
                    (x.Profile.FullName != null && x.Profile.FullName.ToLower().Contains(searchTerm)) ||
                    (x.Profile.FullNameUnsigned != null && x.Profile.FullNameUnsigned.Contains(unsignedTerm)) ||
                    (x.UserName != null && x.UserName.Contains(unsignedTerm)) ||
                    (x.UserNameUnsigned != null && x.UserNameUnsigned.Contains(unsignedTerm)) ||
                    (x.Email != null && x.Email.Contains(unsignedTerm))
                );
            }

            if (request.Status.HasValue)
            {
                query = query.Where(x => x.UserStatus == request.Status);
            }

            // ---- Sort ----
            query = request.SortBy switch
            {
                SortBy.IDASC => query.OrderBy(x => x.Id),
                SortBy.IDDESC => query.OrderByDescending(x => x.Id),
                SortBy.NAMEASC => query.OrderBy(x => x.Profile.FullName),
                SortBy.NAMEDESC => query.OrderByDescending(x => x.Profile.FullName),
                SortBy.DATEASC => query.OrderBy(x => x.UpdatedAt.ToString() ?? x.CreatedAt.ToString()),
                SortBy.DATEDESC => query.OrderByDescending(x => x.UpdatedAt.ToString() ?? x.CreatedAt.ToString()),
                _ => query.OrderBy(x => x.Id)
            };

            // ---- Paging ----

            var dtoQuery = query.ProjectTo<UserSearchResponse>(mapper.ConfigurationProvider);
            var paged = await dtoQuery.ToPagedResponseAsync(request.Page, request.PageSize);


            return paged;
        }

        public async Task<UserDetailResponse> GetUserDetailForAdmin(int id)
        {
            var accountIdClaim = httpContextAccessor.HttpContext?.User.FindFirst("userId")?.Value;
            if (string.IsNullOrEmpty(accountIdClaim))
                throw new AppException(ErrorCode.UNAUTHORIZED);

            var userOptinal = await userRepository.FindUserByIdWithAllRelations(id);
            if (userOptinal == null)
            {
                throw new AppException(ErrorCode.USER_NOT_EXISTED);
            }

            var response = mapper.Map<UserDetailResponse>(userOptinal);

            return response;
        }

        public async Task<UserCreationResponse> CreateUserForAdmin(UserCreationByAdminRequest request)
        {
            var accountIdClaim = httpContextAccessor.HttpContext?.User.FindFirst("userId")?.Value;
            if (string.IsNullOrEmpty(accountIdClaim))
                throw new AppException(ErrorCode.UNAUTHORIZED);

            var userOptinal = await userRepository.FindUserByEmail(request.Email);
            if (userOptinal != null)
            {
                logger.LogError("User Existed");
                throw new AppException(ErrorCode.USER_EXISTED);
            }

            User user = mapper.Map<User>(request);
            var generatedPassword = PasswordHelper.GenerateRandomPassword(8);
            user.PasswordHash = passwordHasher.HashPassword(user, generatedPassword);
            user.UserStatus = UserStatus.PENDING_APPROVE;

            var role = await roleRepository.FindByRoleName(request.RoleName.ToUpper());
            if (role == null)
            {
                role = new Role();
                role.Name = request.RoleName;
                await roleRepository.CreateRole(role);
            }
            user.RoleId = role.Id;
            user.CreatedBy = accountIdClaim;
            await userRepository.AddAsync(user);

            var profile = new Models.Profile
            {
                UserId = user.Id,
                FullName = request.FullName,
                Address = request.Address,
                DateOfBirth = request.DateOfBirth,
                Phone = request.Phone,               
                AvatarUrl = "https://res.cloudinary.com/dea92gqx4/image/upload/v1761033759/Windows_10_Default_Profile_Picture.svg_x71ugm.png",
                CreatedBy = accountIdClaim
            };

            await profileRepository.AddAsync(profile);

            var userPoint = new UserPoint
            {
                UserId = user.Id,
            };

            await userPointRepository.AddAsync(userPoint);

            if (request.RoleName.ToUpper() == DefinitionRole.STAFF)
            {
                var staff = new Staff
                {
                    UserId = user.Id,
                    StaffRole = request.StaffRole,
                    CanManageEvents = request.CanManageEvents,
                    CanAssignTasks = request.CanAssignTasks,
                    CanReplyReports = request.CanReplyReports,
                    CreatedBy = accountIdClaim,
                    StaffStatus = StaffStatus.PENDING

                };

                await staffRepository.AddAsync(staff);
            }
            if (request.RoleName.ToUpper() == DefinitionRole.CONTRIBUTOR)
            {
                var contributor = new Contributor
                {
                    UserId = user.Id,
                    Bio = request.Bio,
                    Expertise = request.Expertise,
                    IsPremiumEligible = request.IsPremiumEligible,
                    CreatedBy = accountIdClaim,     
                    Status = ContributorStatus.APPLIED
                };

                await contributorRepository.AddAsync(contributor);
            }

            await mailService.SendEmailWelcomeForAdmin(user.Email, profile.FullName, generatedPassword, ToVietnamese(user.Role.Name), user.UserName);

            return mapper.Map<UserCreationResponse>(user);
        }

        public async Task<bool> ChangeUserStatusForAdmin(int id, UserStatus status)
        {
            var accountIdClaim = httpContextAccessor.HttpContext?.User.FindFirst("userId")?.Value;
            if (string.IsNullOrEmpty(accountIdClaim))
                throw new AppException(ErrorCode.UNAUTHORIZED);

            var userOptinal = await userRepository.FindUserById(id);
            if (userOptinal == null)
            {
                throw new AppException(ErrorCode.USER_NOT_EXISTED);
            }
            userOptinal.UserStatus = status;
            userOptinal.UpdatedAt = DateTime.UtcNow;
            userOptinal.UpdatedBy = accountIdClaim;

            await userRepository.UpdateAsync(userOptinal);

            return true;
        }
        private string ToVietnamese(string role)
        {
            return role switch
            {
                "GUEST" => "Khách",
                "MEMBER" => "Thành viên",
                "CONTRIBUTOR" => "Người đóng góp nội dung",
                "STAFF" => "Nhân viên kiểm duyệt",
                "ADMIN" => "Quản trị viên",
                _ => "Người dùng"
            };
        }

        public async Task<User> GetRandomUserExcept(int userId)
        {
            // Lấy danh sách ID để không giữ IQueryable quá lâu
            var ids = await userRepository.GetQueryable()
                .Where(u => u.Id != userId && u.Role.Name == DefinitionRole.MEMBER)
                .Select(u => u.Id)
                .ToListAsync();

            if (ids.Count == 0) return null;

            var rnd = new Random();
            var randomId = ids[rnd.Next(ids.Count)];

            // Lấy đúng user theo ID (truy vấn ngắn, sạch)
            return await userRepository.GetQueryable()
                .FirstOrDefaultAsync(u => u.Id == randomId);
        }


    }
}

using Azure.Core;
using Cultural_Heritage_System.Common;
using Cultural_Heritage_System.Dtos.Request;
using Cultural_Heritage_System.Middlewares;
using Cultural_Heritage_System.Models;
using Cultural_Heritage_System.Repositories;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace Cultural_Heritage_System.Services.Impl
{
    public class AuthService : IAuthService
    {
        private readonly IHttpContextAccessor httpContextAccessor;
        private readonly ILogger<AuthenticationService> logger;
        private readonly IJwtService jwtService;
        private readonly PasswordHasher<User> passwordHasher;
        private readonly UserRepository userRepository;
        private readonly RoleRepository roleRepository;
        private readonly GoogleAuthClient googleAuthClient;
        private readonly GoogleUserInfoClient googleUserInfoClient;
        private readonly ProfileRepository profileRepository;
        private readonly RefreshTokenRepository refreshTokenRepository;
        //private readonly FacebookAuthClient facebookAuthClient;
        //private readonly FacebookUserInfoClient facebookUserInfoClient;

        public AuthService(
            ILogger<AuthenticationService> logger,
            IJwtService jwtService,
            UserRepository userRepository,
            RoleRepository roleRepository,
            GoogleAuthClient googleAuthClient,
            GoogleUserInfoClient googleUserInfoClient,
            ProfileRepository profileRepository,
            RefreshTokenRepository refreshTokenRepository,
            IHttpContextAccessor httpContextAccessor
            )
        {
            this.logger = logger;
            this.jwtService = jwtService;
            passwordHasher = new PasswordHasher<User>();
            this.userRepository = userRepository;
            this.roleRepository = roleRepository;
            this.googleAuthClient = googleAuthClient;
            this.googleUserInfoClient = googleUserInfoClient;
            this.profileRepository = profileRepository;
            //this.facebookAuthClient = facebookAuthClient;
            //this.facebookUserInfoClient = facebookUserInfoClient;
            this.refreshTokenRepository = refreshTokenRepository;
            this.httpContextAccessor = httpContextAccessor;
        }

        public async Task<SignInResponse> SignIn(SignInRequest request)
        {
            logger.LogInformation("SignIn start ...");

            var user = await userRepository.FindUserByEmailOrUserName(request.EmailOrUsername);

            if (user == null)
            {
                logger.LogError("SignIn Failed: User not found.");
                throw new AppException(ErrorCode.USER_NOT_EXISTED);
            }


            var passwordVerificationResult = passwordHasher.VerifyHashedPassword(user, user.PasswordHash, request.Password);

            if (passwordVerificationResult != PasswordVerificationResult.Success)
            {
                logger.LogError("SignIn Failed: Invalid password.");
                throw new AppException(ErrorCode.UNAUTHORIZED);
            }

            if (user.UserStatus == UserStatus.INACTIVE)
            {
                throw new AppException(ErrorCode.ACCOUNT_LOCKED);
            }

            if (user.UserStatus == UserStatus.PENDING_VERIFICATION)
            {

                return new SignInResponse(TwoFaStep.SETUP_REQUIRED);
            }
            
            if (user.Enable2FA)
            {
            
                return new SignInResponse(TwoFaStep.VERIFICATION_REQUIRED);
            }

            var claims = new[]
            {
                new Claim("userId", user.Id.ToString()),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim("Authorities", user.Role.Name)
            };

            var accessToken = jwtService.GenerateAccessToken(claims);
            var refreshToken = jwtService.GenerateRefreshToken(claims);

            await refreshTokenRepository.AddAsync(new RefreshToken
            {
                UserId = user.Id,
                Token = refreshToken
            });

            logger.LogInformation("SignIn success for userId: {UserId}", user.Id);

            return new SignInResponse(accessToken, refreshToken, user.Role.Name, "Bearer", TwoFaStep.NONE, user.UserName,user.Profile.AvatarUrl);
        }

        public async Task<SignInResponse> SignInWithGoogle(string code)
        {

            var token = await googleAuthClient.ExchangeTokenAsync(code);
            var userInfo = await googleUserInfoClient.GetUserInfoAsync(token.AccessToken);

            var user = await userRepository.FindUserByEmail(userInfo.Email);

            if (user == null)
            {
                var role = await roleRepository.FindByRoleName(DefinitionRole.MEMBER);
                if (role == null)
                {
                    role = new Role();
                    role.Name = DefinitionRole.MEMBER;
                    await roleRepository.CreateRole(role);
                }
                user = new User
                {
                    UserName = userInfo.Name,
                    
                    Email = userInfo.Email,
                    Role = role,
                    PasswordHash = passwordHasher.HashPassword(user, "123")
                };
                await userRepository.AddAsync(user);


                Profile profile = new Profile
                {
                    UserId = user.Id,
                    FullName = userInfo.Name,
                    AvatarUrl = userInfo.Picture,
                };
                await profileRepository.AddAsync(profile);

            }

            var claims = new[]
            {
                new Claim("userId", user.Id.ToString()),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim("Authorities", user.Role.Name)
            };

            var accessToken = jwtService.GenerateAccessToken(claims);
            var refreshToken = jwtService.GenerateRefreshToken(claims);

            await refreshTokenRepository.AddAsync(new RefreshToken
            {
                UserId = user.Id,
                Token = refreshToken
            });


            logger.LogInformation("SignIn Google success for userId: {UserId}", user.Id);

            return new SignInResponse(accessToken, refreshToken, user.Role.Name, "Bearer", TwoFaStep.NONE,user.UserName,user.Profile.AvatarUrl);
        }


        //public async Task<SignInResponse> SignInWithFacebook(string code)
        //{
        //    var exchangeToken = await facebookAuthClient.ExchangeTokenAsync(code);
        //    if (exchangeToken == null)
        //    {
        //        throw new ArgumentException("Exchange token is null");
        //    }
        //    var userInfo = await facebookUserInfoClient.GetUserProfileAsync(exchangeToken.AccessToken);

        //    if (userInfo == null)
        //    {
        //        throw new ArgumentException("Profile is null");
        //    }

        //    var email = userInfo.Email;
        //    if (string.IsNullOrEmpty(email))
        //    {
        //        email = GenerateUniqueEmail("facebook");
        //        logger.LogWarning("Email không có sẵn, đã sinh email tạm: {Email}", email);
        //    }

        //    var user = await userRepository.FindUserByEmail(email);

        //    if (user == null)
        //    {
        //        var role = await roleRepository.FindByRoleName(DefinitionRole.USER);
        //        if (role == null)
        //        {
        //            role = new Role();
        //            role.Name = DefinitionRole.USER;
        //            await roleRepository.CreateRole(role);
        //        }
        //        user = new User
        //        {
        //            Email = email,
        //            Role = role
        //        };

        //        await userRepository.CreateUserAsync(user);

        //        Patient patient = new Patient
        //        {
        //            UserId = user.Id,
        //            Avatar = userInfo.Picture.Data.Url,
        //        };
        //        await patientRepository.CreatePatientAsync(patient);
        //    }

        //    var claims = new[]
        //    {
        //    new Claim("userId", user.Id.ToString()),
        //    new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
        //    new Claim("Authorities", user.Role.Name)
        //};

        //    var accessToken = jwtService.GenerateAccessToken(claims);
        //    var refreshToken = jwtService.GenerateRefreshToken(claims);

        //    logger.LogInformation("Đăng nhập bằng Facebook thành công cho userId: {UserId}", user.Id);

        //    return new SignInResponse(accessToken, refreshToken, user.Role.Name, "Bearer", TwoFaStep.NONE);
        //}

        public async Task<SignInResponse> RefreshToken(string refreshToken)
        {
            if (string.IsNullOrWhiteSpace(refreshToken))
                throw new AppException(ErrorCode.UNAUTHORIZED);

        
            var rt = await refreshTokenRepository.FindByTokenAsync(refreshToken);
            if (rt == null) 
                throw new AppException(ErrorCode.UNAUTHORIZED);

          
            var user = await userRepository.FindUserById(rt.UserId);
            if (user == null)
                throw new AppException(ErrorCode.UNAUTHORIZED);

            var claims = new[]
            {
                new Claim("userId", user.Id.ToString()),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim("Authorities", user.Role.Name)
            };

            var accessToken = jwtService.GenerateAccessToken(claims);
            var newRefreshToken = jwtService.GenerateRefreshToken(claims);

            rt.Token = newRefreshToken;

            await refreshTokenRepository.UpdateAsync(rt);

            // 5) TwoFaStep: parse từ field đúng (VD: user.TwoFaStepString)
            TwoFaStep stepEnum = TwoFaStep.NONE;
            if (!string.IsNullOrWhiteSpace(user.TwoFactorSecret))
            {
                if (!Enum.TryParse(user.TwoFactorSecret, ignoreCase: true, out stepEnum))
                    stepEnum = TwoFaStep.NONE;
            }
         
            // 6) Trả response
            return new SignInResponse(
                accessToken,
                newRefreshToken,
                user.Role?.Name ?? "User",
                "Bearer",
                stepEnum,
                user.UserName,
                user.Profile.AvatarUrl
            );
        }


        public Task SignOut()
        {
            throw new NotImplementedException();
        }


        private string GenerateUniqueEmail(string login)
        {
            var uniqueId = Guid.NewGuid().ToString("N"); // "N" để tạo chuỗi không có dấu gạch nối
            return $"{login}-{uniqueId}@generated.com";
        }

        public Task<SignInResponse> SignInWithFacebook(string code)
        {
            throw new NotImplementedException();
        }
    }
}

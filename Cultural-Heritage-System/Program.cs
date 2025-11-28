
using Cultural_Heritage_System.Configuration;
using Cultural_Heritage_System.DataAccessObjects;
using Cultural_Heritage_System.GameHubs;
using Cultural_Heritage_System.Helpers;
using Cultural_Heritage_System.Middlewares;
using Cultural_Heritage_System.Models;
using Cultural_Heritage_System.Repositories;
using Cultural_Heritage_System.Repositories.Impl;
using Cultural_Heritage_System.Services;
using Cultural_Heritage_System.Services.Impl;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.EntityFrameworkCore;
using StackExchange.Redis;

namespace Cultural_Heritage_System
{
    public class Program
    {
        public static void Main(string[] args)
        {

            var builder = WebApplication.CreateBuilder(args);

            builder.Configuration.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);

            builder.Services.AddControllers()
                 .AddJsonOptions(options =>
                 {
                     options.JsonSerializerOptions.DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull;
                 });

            builder.Services.AddHttpClient<GoogleAuthClient>(client =>
            {
                client.BaseAddress = new Uri("https://oauth2.googleapis.com/");
            });

            builder.Services.AddHttpClient<GoogleUserInfoClient>(client =>
            {
                client.BaseAddress = new Uri("https://www.googleapis.com/");
            });


            //builder.Services.AddHttpClient<FacebookAuthClient>(client =>
            //{
            //    client.BaseAddress = new Uri("https://graph.facebook.com/");
            //});

            //builder.Services.AddHttpClient<FacebookUserInfoClient>(client =>
            //{
            //    client.BaseAddress = new Uri("https://graph.facebook.com/");
            //});
            builder.Services.AddSignalR();
            builder.Services.AddCors(options =>
            {
                options.AddPolicy("AllowReactApp", policy =>
                {
                    policy.WithOrigins("http://localhost:3000", "https://localhost:3000", "https://heritage-web-ashy.vercel.app")
                          .AllowAnyHeader()
                          .AllowAnyMethod()
                          .AllowCredentials(); 
                });
            });

            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("DefaultConnection"),
        sqlOptions => sqlOptions.CommandTimeout(120)
    )
);


            builder.Services.AddCustomJwtAuthentication(builder.Configuration); // JWT
            CorsConfiguration.ConfigureServices(builder.Services); // CORS
            builder.Services.AddHttpContextAccessor(); // HttpContextAccessor

            builder.Services.Configure<FormOptions>(options =>
            {
                options.MultipartBodyLengthLimit = 104857600; // 100MB
            });
            builder.Services.AddScoped<UserDAO>();
            builder.Services.AddScoped<TagDAO>();
            builder.Services.AddScoped<CategoryDAO>();
            builder.Services.AddScoped<RoleDAO>();
            builder.Services.AddScoped<ProfileDAO>();
            builder.Services.AddScoped<PasswordResetDAO>();
            builder.Services.AddScoped<HeritageDAO>();
            builder.Services.AddScoped<ReportDAO>();
            builder.Services.AddScoped<ReviewDAO>();
            builder.Services.AddScoped<RefreshTokenDAO>();
            builder.Services.AddScoped<HeritageOccurrenceDAO>();
            builder.Services.AddScoped<HeritageTagDAO>();
            builder.Services.AddScoped<HeritageLocationDAO>();
            builder.Services.AddScoped<HeritageMediaDAO>();
            builder.Services.AddScoped<LocationDAO>();
            builder.Services.AddScoped<FavoriteDAO>();
            builder.Services.AddScoped<HeritageDAO>();
            builder.Services.AddScoped<ReportReplyDAO>();
            builder.Services.AddScoped<ContributorDAO>();
            builder.Services.AddScoped<ContributionDAO>();
            builder.Services.AddScoped<SubscriptionDAO>();
            builder.Services.AddScoped<ContributionAccessLogDAO>();     
            builder.Services.AddScoped<ContributionSaveDAO>();
            builder.Services.AddScoped<ContributionReviewDAO>();
            builder.Services.AddScoped<ContributionHeritageTagDAO>();
            builder.Services.AddScoped<ContributionReportDAO>();
            builder.Services.AddScoped<StaffDAO>();
            builder.Services.AddScoped<ContributionAcceptanceDAO>();
            builder.Services.AddScoped<QuizDAO>();
            builder.Services.AddScoped<QuizResultDAO>();
            builder.Services.AddScoped<QuizQuestionDAO>();
            builder.Services.AddScoped<PanoramaTourDAO>();
            builder.Services.AddScoped<PanoramaSceneDAO>();
            builder.Services.AddScoped<ContributionUnlockDAO>();
            builder.Services.AddScoped<SubscriptionUsageDAO>();
            builder.Services.AddScoped<PanoramaInteractionPointDAO>();
            builder.Services.AddScoped<PremiumPackageDAO>();
            builder.Services.AddScoped<PremiumBenefitDAO>();

            builder.Services.AddScoped<IUserRepository, UserRepository>();
            builder.Services.AddScoped<ITagRepository, TagRepository>();
            builder.Services.AddScoped<ICategoryRepository, CategoryRepository>();
            builder.Services.AddScoped<IRoleRepository, RoleRepository>();
            builder.Services.AddScoped<IProfileRepository, ProfileRepository>();
            builder.Services.AddScoped<IPasswordResetRepository, PasswordResetRepository>();
            builder.Services.AddScoped<IHeritageRepository, HeritageRepository>();
            builder.Services.AddScoped<IReportRepository, ReportRepository>();
            builder.Services.AddScoped<IReviewRepository, ReviewRepository>();
            builder.Services.AddScoped<IRefreshTokenRepository, RefreshTokenRepository>();
            builder.Services.AddScoped<IHeritageOccurrenceRepository, HeritageOccurrenceRepository>();
            builder.Services.AddScoped<IHeritageTagRepository, HeritageTagRepository>();
            builder.Services.AddScoped<IHeritageLocationRepository, HeritageLocationRepository>();
            builder.Services.AddScoped<IHeritageMediaRepository, HeritageMediaRepository>();
            builder.Services.AddScoped<ILocationRepository, LocationRepository>();
            builder.Services.AddScoped<IFavoriteRepository, FavoriteRepository>();
            builder.Services.AddScoped<IHeritageRepository, HeritageRepository>();          
            builder.Services.AddScoped<IReportReplyRepository, ReportReplyRepository>();
            builder.Services.AddScoped<IContributorRepository, ContributorRepository>();
            builder.Services.AddScoped<IContributionRepository, ContributionRepository>();
            builder.Services.AddScoped<ISubscriptionRepository, SubscriptionRepository>();
            builder.Services.AddScoped<IContributionAccessLogRepository, ContributionAccessLogRepository>();        
            builder.Services.AddScoped<IContributionSaveRepository, ContributionSaveRepository>();
            builder.Services.AddScoped<IContributionReviewRepository, ContributionReviewRepository>();
            builder.Services.AddScoped<IContributionHeritageTagRepository, ContributionHeritageTagRepository>();
            builder.Services.AddScoped<IContributionReportRepository, ContributionReportRepository>();
            builder.Services.AddScoped<IStaffRepository, StaffRepository>();
            builder.Services.AddScoped<IContributionAcceptanceRepository, ContributionAcceptanceRepository>();
            builder.Services.AddScoped<IQuizRepository, QuizRepository>();
            builder.Services.AddScoped<IQuizResultRepository, QuizResultRepository>();
            builder.Services.AddScoped<IQuizQuestionRepository, QuizQuestionRepository>();
            builder.Services.AddScoped<IPanoramaTourRepository, PanoramaTourRepository>();
            builder.Services.AddScoped<IPanoramaSceneRepository, PanoramaSceneRepository>();
            builder.Services.AddScoped<IContributionUnlockRepository, ContributionUnlockRepository>();
            builder.Services.AddScoped<ISubscriptionUsageRepository, SubscriptionUsageRepository>();
            builder.Services.AddScoped<IPanoramaInteractionPointRepository, PanoramaInteractionPointRepository>();
            builder.Services.AddScoped<IPremiumPackageRepository, PremiumPackageRepository>();
            builder.Services.AddScoped<IPremiumBenefitRepository, PremiumBenefitRepository>();

            builder.Services.AddScoped<IUserService, UserService>();
            builder.Services.AddScoped<ITagService, TagService>();
            builder.Services.AddScoped<ICategoryService, CategoryService>();
            builder.Services.AddScoped<IAuthService, AuthService>();
            builder.Services.AddScoped<IJwtService, JwtService>();
            builder.Services.AddScoped<IMailService, MailService>();
            builder.Services.AddScoped<IPasswordResetService, PasswordResetService>();
            builder.Services.AddScoped<IFavoriteService, FavoriteService>();           
            //builder.Services.AddScoped<IProfileService, ProfileService>();
            builder.Services.AddScoped<ICloudinaryService, CloudinaryService>();
            builder.Services.AddScoped<IHeritageService, HeritageService>();
            builder.Services.AddScoped<IReportService, ReportService>();
            builder.Services.AddScoped<IContributorService, ContributorService>();
            builder.Services.AddScoped<IReviewService, ReviewService>();
            builder.Services.AddScoped<IReportReplyService, ReportReplyService>();
            builder.Services.AddScoped<IContributionService, ContributionService>();
            builder.Services.AddScoped<IContributionReviewService, ContributionReviewService>();
            builder.Services.AddScoped<IUserCacheService, UserCacheService>();
            builder.Services.AddScoped<IContributionAcceptanceService, ContributionAcceptanceService>();
            builder.Services.AddScoped<IQuizService, QuizService>();
            builder.Services.AddScoped<IPanoramaTourService, PanoramaTourService>();
            builder.Services.AddScoped<IStaffService, StaffService>();
            builder.Services.AddScoped<IPremiumPackageService, PremiumPackageService>();
            builder.Services.AddScoped<IPremiumBenefitService, PremiumBenefitService>();

            // Redis
            builder.Services.AddSingleton<IConnectionMultiplexer>(sp =>
            {
                var configuration = new ConfigurationOptions
                {
                    EndPoints = { builder.Configuration["Redis:Host"] + ":" + builder.Configuration["Redis:Port"] },
                    DefaultDatabase = int.Parse(builder.Configuration["Redis:DefaultDatabase"] ?? "0"),
                    Password = builder.Configuration["Redis:Password"],
                    AbortOnConnectFail = false 
                };
                return ConnectionMultiplexer.Connect(configuration);
            });

            //builder.Services.AddScoped<IUserCacheService, UserCacheService>();


            builder.Services.AddAutoMapper(typeof(MappingProfile));
            builder.Services.AddScoped<TwoFactorService>();

            var app = builder.Build();

            using (var scope = app.Services.CreateScope())
            {
                var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
                //dbContext.Database.Migrate();
            }

            //if (app.Environment.IsDevelopment())
            //{
                app.UseSwagger();
                app.UseSwaggerUI();
            //    app.UseDeveloperExceptionPage();
            //}
            //else
            //{
            //    app.UseExceptionHandler("/error");
            //}

            app.UseHttpsRedirection();
            app.UseCors("AllowReactApp");


            app.UseMiddleware<ExceptionMiddleware>();
            app.UseAuthentication();
            app.UseAuthorization();



            app.MapControllers();

            app.MapHub<GameHub>("/gamehub");
            app.Run();
        }
    }
}

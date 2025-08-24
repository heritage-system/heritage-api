
using Cultural_Heritage_System.Configuration;
using Cultural_Heritage_System.Helpers;
using Cultural_Heritage_System.Middlewares;
using Cultural_Heritage_System.Models;
using Cultural_Heritage_System.Repositories;
using Cultural_Heritage_System.Services;
using Cultural_Heritage_System.Services.Impl;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

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

            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            builder.Services.AddDbContext<AppDbContext>(options =>
               options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

            builder.Services.AddCustomJwtAuthentication(builder.Configuration); // JWT
            CorsConfiguration.ConfigureServices(builder.Services); // CORS
            builder.Services.AddHttpContextAccessor(); // HttpContextAccessor

            builder.Services.AddScoped<UserRepository>();
            builder.Services.AddScoped<RoleRepository>();
            builder.Services.AddScoped<ProfileRepository>();
            builder.Services.AddScoped<PasswordResetRepository>();
            builder.Services.AddScoped<HeritageLocationRepository>();

            builder.Services.AddScoped<IUserService, UserService>();
            builder.Services.AddScoped<IAuthService, AuthService>();
            builder.Services.AddScoped<IJwtService, JwtService>();
            builder.Services.AddScoped<IMailService, MailService>();
            builder.Services.AddScoped<IPasswordResetService, PasswordResetService>();
            builder.Services.AddScoped<IHeritageLocationService, HeritageLocationService>();

            //builder.Services.AddScoped<IProfileService, ProfileService>();

            builder.Services.AddAutoMapper(typeof(MappingProfile));
            builder.Services.AddScoped<TwoFactorService>();

            var app = builder.Build();

            using (var scope = app.Services.CreateScope())
            {
                var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
                dbContext.Database.Migrate();
            }

            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();
            app.UseCors("AllowAll");

            app.UseMiddleware<ExceptionMiddleware>();
            app.UseAuthentication();
            app.UseAuthorization();


            app.MapControllers();

            app.Run();
        }
    }
}

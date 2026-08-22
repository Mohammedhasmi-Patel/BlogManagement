using BlogManagement.Configurations;
using BlogManagement.Database;
using BlogManagement.Models;
using BlogManagement.ServiceContracts;
using BlogManagement.Services;
using FluentValidation;
using Mapster;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace BlogManagement.Extension
{
    public static class ConfigureProjectServices
    {
        public static IServiceCollection ConfigureProjectService(this IServiceCollection service, IConfiguration configuration)
        {

            service.Configure<JwtConfiguration>(configuration.GetSection("JwtConfiguration"));
            service.Configure<FileUploadSettings>(configuration.GetSection("FileUpload"));
            service.Configure<AppSettings>(configuration.GetSection("AppSettings"));

            JwtConfiguration jwtConfiguration = configuration.GetSection("JwtConfiguration").Get<JwtConfiguration>()!;
            service.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            }).AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters()
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateIssuerSigningKey = true,
                    ValidateLifetime = true,
                    ValidIssuer = jwtConfiguration.Issuer,
                    ValidAudience = jwtConfiguration.Audience,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtConfiguration.SecretKey))
                };
            });
            service.AddControllers();
            string databaseUrl = configuration.GetConnectionString("DefaultConnection")!;

            service.AddSwaggerGen();
            service.AddDbContext<AppDbContext>(options => options.UseSqlServer(databaseUrl));
            service.AddIdentity<AppUser, AppRole>()
                    .AddEntityFrameworkStores<AppDbContext>()
                    .AddDefaultTokenProviders();

            service.AddValidatorsFromAssembly(typeof(ConfigureProjectServices).Assembly);

            service.AddMapster();

            service.AddScoped<IAuthService, AuthService>();
            service.AddScoped<IFileStorageService,FileStorageService>();
            service.AddScoped<ITokenService, TokenService>();


            return service;
        }
    }
}

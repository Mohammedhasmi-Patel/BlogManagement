using System.Text;
using BlogManagement.Configurations;
using BlogManagement.Database;
using BlogManagement.DTO.Common;
using BlogManagement.Models;
using BlogManagement.ServiceContracts;
using BlogManagement.Services;
using FluentValidation;
using Mapster;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi;

namespace BlogManagement.Extension;

public static class ConfigureProjectServices
{
    public static IServiceCollection ConfigureProjectService(this IServiceCollection service, IConfiguration configuration)
    {

        service.Configure<JwtConfiguration>(configuration.GetSection("JwtConfiguration"));
        service.Configure<FileUploadSettings>(configuration.GetSection("FileUpload"));
        service.Configure<AppSettings>(configuration.GetSection("AppSettings"));

        JwtConfiguration jwtConfiguration = configuration.GetSection("JwtConfiguration").Get<JwtConfiguration>()!;
        List<string> allowedOrigins = configuration.GetSection("AllowedOrigins").Get<List<string>>()!;

        service.AddCors(options =>
        {
            options.AddPolicy("FrontendCors", policy => policy
            .WithOrigins(allowedOrigins.ToArray()!)
            .AllowAnyMethod()
            .AllowAnyHeader());
        });

        service.AddControllers();
        string databaseUrl = configuration.GetConnectionString("DefaultConnection")!;
        service.AddSwaggerGen(options =>
        {
            options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
            {
                In = ParameterLocation.Header,
                Description = "Please enter a valid JWT token",
                Name = "Authorization",
                Type = SecuritySchemeType.Http,
                BearerFormat = "JWT",
                Scheme = "Bearer"
            });

            options.AddSecurityRequirement(document => new OpenApiSecurityRequirement
            {
                {
                    new OpenApiSecuritySchemeReference("Bearer", document),
                    new List<string>()
                }
            });
        });

        service.AddDbContext<AppDbContext>(options => options.UseSqlServer(databaseUrl));

        service.AddIdentityCore<AppUser>()
                .AddRoles<AppRole>()
                .AddEntityFrameworkStores<AppDbContext>()
                .AddDefaultTokenProviders();

        service.AddAuthentication(options =>
        {
            options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
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

            options.Events = new JwtBearerEvents
            {
                OnChallenge = async context =>
                {
                    context.HandleResponse();
                    context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                    context.Response.ContentType = "application/json";

                    var response = ApiResponse<object>.ErrorResponse(StatusCodes.Status401Unauthorized, "Unauthorized.");
                    await context.Response.WriteAsJsonAsync(response);
                },
                OnForbidden = async context =>
                {
                    context.Response.StatusCode = StatusCodes.Status403Forbidden;
                    context.Response.ContentType = "application/json";

                    var response = ApiResponse<object>.ErrorResponse(
                        StatusCodes.Status403Forbidden,
                        "You do not have permission to access this resource."
                    );

                    await context.Response.WriteAsJsonAsync(response);
                }
            };

        });

        service.AddValidatorsFromAssembly(typeof(ConfigureProjectServices).Assembly);

        service.AddMapster();

        service.AddScoped<IAuthService, AuthService>();
        service.AddScoped<IFileStorageService, FileStorageService>();
        service.AddScoped<ITokenService, TokenService>();
        service.AddScoped<ICategoryService, CategoryService>();
        service.AddScoped<IBlogService, BlogService>();
        service.AddScoped<IBookmarkService, BookmarkService>();
        service.AddScoped<ILikeService, LikeService>();
        service.AddScoped<ICommentService, CommentService>();

        return service;
    }

}

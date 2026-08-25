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
    public static IServiceCollection ConfigureProjectService(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddAppConfigurations(configuration);
        services.AddAppCors(configuration);
        services.AddAppDatabaseAndIdentity(configuration);
        services.AddAppAuthentication(configuration);
        services.AddAppSwagger();
        services.AddAppValidationAndMapping();
        services.AddAppServices();

        services.AddControllers();

        return services;
    }

    private static IServiceCollection AddAppConfigurations(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<JwtConfiguration>(configuration.GetSection("JwtConfiguration"));
        services.Configure<FileUploadSettings>(configuration.GetSection("FileUpload"));
        services.Configure<AppSettings>(configuration.GetSection("AppSettings"));

        return services;
    }

    private static IServiceCollection AddAppCors(this IServiceCollection services, IConfiguration configuration)
    {
        List<string> allowedOrigins = configuration.GetSection("AllowedOrigins").Get<List<string>>() ?? [];

        services.AddCors(options =>
        {
            options.AddPolicy("FrontendCors", policy => policy
                .WithOrigins([.. allowedOrigins])
                .AllowAnyMethod()
                .AllowAnyHeader());
        });

        return services;
    }

    private static IServiceCollection AddAppDatabaseAndIdentity(this IServiceCollection services, IConfiguration configuration)
    {
        string databaseUrl = configuration.GetConnectionString("DefaultConnection")!;

        services.AddDbContext<AppDbContext>(options => options.UseSqlServer(databaseUrl));

        services.AddIdentityCore<AppUser>()
            .AddRoles<AppRole>()
            .AddEntityFrameworkStores<AppDbContext>()
            .AddDefaultTokenProviders();

        return services;
    }

    private static IServiceCollection AddAppAuthentication(this IServiceCollection services, IConfiguration configuration)
    {
        JwtConfiguration jwtConfiguration = configuration.GetSection("JwtConfiguration").Get<JwtConfiguration>()!;

        services.AddAuthentication(options =>
        {
            options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
        }).AddJwtBearer(options =>
        {
            options.TokenValidationParameters = new TokenValidationParameters
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

        return services;
    }

    private static IServiceCollection AddAppSwagger(this IServiceCollection services)
    {
        services.AddSwaggerGen(options =>
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

        return services;
    }

    private static IServiceCollection AddAppValidationAndMapping(this IServiceCollection services)
    {
        services.AddValidatorsFromAssembly(typeof(ConfigureProjectServices).Assembly);
        services.AddMapster();

        return services;
    }

    private static IServiceCollection AddAppServices(this IServiceCollection services)
    {
        services.AddScoped<IAuthService, AuthService>();
        services.AddScoped<ITokenService, TokenService>();
        services.AddScoped<IFileStorageService, FileStorageService>();
        services.AddScoped<ICategoryService, CategoryService>();
        services.AddScoped<IBlogService, BlogService>();
        services.AddScoped<IBookmarkService, BookmarkService>();
        services.AddScoped<ILikeService, LikeService>();
        services.AddScoped<ICommentService, CommentService>();
        services.AddScoped<IUserFollowService, UserFollowService>();

        return services;
    }
}

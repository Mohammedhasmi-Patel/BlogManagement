using BlogManagement.Database;
using BlogManagement.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace BlogManagement.Extension
{
    public static class ConfigureProjectServices
    {
        public static IServiceCollection ConfigureProjectService(this IServiceCollection service, IConfiguration configuration)
        {
            service.AddControllers();
            string databaseUrl = configuration.GetConnectionString("DefaultConnection")!;
            service.AddDbContext<AppDbContext>(options =>
            {
                options.UseSqlServer(databaseUrl);
            });

            service.AddIdentity<AppUser, AppRole>()
                    .AddEntityFrameworkStores<AppDbContext>()
                    .AddDefaultTokenProviders();
            return service;
        }
    }
}

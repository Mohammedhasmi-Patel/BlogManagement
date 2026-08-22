using BlogManagement.Models;
using Microsoft.AspNetCore.Identity;

namespace BlogManagement.Seeders
{
    public static class DatabaseSeeder
    {
        public static async Task SeedAsync(IServiceProvider serviceProvider)
        {
            using var scope = serviceProvider.CreateScope();

            var appRole = scope.ServiceProvider.GetRequiredService<RoleManager<AppRole>>();

            //string [] roles = System.Enum.GetNames<UserRoleEnum>();
            await RoleSeeder.SeedAsync(appRole);


        }
    }
}

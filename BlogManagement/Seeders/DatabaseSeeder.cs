using BlogManagement.Database;
using BlogManagement.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace BlogManagement.Seeders
{
    public static class DatabaseSeeder
    {
        public static async Task SeedAsync(IServiceProvider serviceProvider)
        {
            using var scope = serviceProvider.CreateScope();
            var services = scope.ServiceProvider;

            var context = services.GetRequiredService<AppDbContext>();
            var roleManager = services.GetRequiredService<RoleManager<AppRole>>();
            var userManager = services.GetRequiredService<UserManager<AppUser>>();

            // 1. Ensure Database & Migrations are applied
            if (context.Database.IsRelational())
            {
                await context.Database.MigrateAsync();
            }
            else
            {
                await context.Database.EnsureCreatedAsync();
            }

            // 2. Seed Roles
            await RoleSeeder.SeedAsync(roleManager);

            // 3. Seed Users
            var users = await UserSeeder.SeedAsync(userManager);

            // 4. Seed Categories
            var categories = await CategorySeeder.SeedAsync(context, users);

            // 5. Seed Blogs
            var blogs = await BlogSeeder.SeedAsync(context, users, categories);

            // 6. Seed Interactions (Comments, Likes, Bookmarks, Follows)
            await InteractionSeeder.SeedAsync(context, users, blogs);
        }
    }
}

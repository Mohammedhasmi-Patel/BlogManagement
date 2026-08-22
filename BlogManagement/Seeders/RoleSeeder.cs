using BlogManagement.Enum;
using BlogManagement.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;

namespace BlogManagement.Seeders
{
    public static class RoleSeeder
    {
        public static async Task SeedAsync(RoleManager<AppRole> roleManager)
        {
            if (await roleManager.Roles.AnyAsync()) return;
            string [] roles = System.Enum.GetNames<UserRoleEnum>();

            foreach (string role in roles)
            {
                if(!await roleManager.RoleExistsAsync(role))
                {
                    await roleManager.CreateAsync(new AppRole
                    {
                        Name = role,
                        Description = role,
                    });
                }
            }
        }
    }
}

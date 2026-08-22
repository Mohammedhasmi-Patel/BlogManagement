using BlogManagement.Configurations;
using BlogManagement.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;

namespace BlogManagement.Extension
{
    public static class AppUserExtension
    {
        public static async Task<string> GetUserRole(this AppUser user,UserManager<AppUser> userManager)
        {
            string role = (await userManager.GetRolesAsync(user)).FirstOrDefault()!;
            return role;
        }

        public static async Task<string?> GetUserProfileUrl(this AppUser user,IOptions<AppSettings> setting)
        {
            AppSettings settings = setting.Value;
            return user.Avatar == null ? null : $"{settings.BaseUrl}{user.Avatar}";
        }
    }
}

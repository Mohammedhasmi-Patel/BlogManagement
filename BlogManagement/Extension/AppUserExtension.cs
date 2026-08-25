using BlogManagement.Configurations;
using BlogManagement.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;

namespace BlogManagement.Extension
{
    public static class AppUserExtension
    {
        public static async Task<string> GetUserRole(this AppUser user, UserManager<AppUser> userManager)
        {
            var roles = await userManager.GetRolesAsync(user);
            return roles.FirstOrDefault() ?? "User";
        }

        public static string? GetUserProfileUrl(this AppUser user, IOptions<AppSettings> setting)
        {
            if (string.IsNullOrWhiteSpace(user.Avatar))
            {
                return null;
            }

            if (user.Avatar.StartsWith("http://", StringComparison.OrdinalIgnoreCase) ||
                user.Avatar.StartsWith("https://", StringComparison.OrdinalIgnoreCase))
            {
                return user.Avatar;
            }

            var baseUrl = setting.Value.BaseUrl?.TrimEnd('/') ?? string.Empty;
            var avatarPath = user.Avatar.StartsWith('/') ? user.Avatar : $"/{user.Avatar}";
            return $"{baseUrl}{avatarPath}";
        }
    }
}

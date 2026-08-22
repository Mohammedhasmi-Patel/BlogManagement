using BlogManagement.Database;
using BlogManagement.Models;
using Microsoft.EntityFrameworkCore;

namespace BlogManagement.Extension;

public static class CategoryExtension
{
    public async static Task<string> GenerateUniqueSlug(this Category category,AppDbContext context)
    {
        string baseSlug = category.Name
                .ToLowerInvariant()
                .Trim()
                .Replace(" ", "-");
        string slug = baseSlug;
        int count = 1;

        while (await context.Categories.AnyAsync(c => c.Slug == slug))
        {
             slug = $"{baseSlug}-{count}";
             count++;
        }

        return slug;
    }

}

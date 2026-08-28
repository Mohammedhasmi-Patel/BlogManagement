using System.Text.RegularExpressions;
using BlogManagement.Database;
using BlogManagement.Models;
using Microsoft.EntityFrameworkCore;

namespace BlogManagement.Extension;

public static class CategoryExtension
{
    public static async Task<string> GenerateUniqueSlug(this Category category, AppDbContext context, CancellationToken ct = default)
    {
        string raw = (category.Name ?? string.Empty).ToLowerInvariant().Trim();

        // Remove invalid characters (keep lowercase letters, digits, spaces, hyphens)
        string cleaned = Regex.Replace(raw, @"[^a-z0-9\s-]", string.Empty);

        // Replace consecutive whitespace/hyphens with a single hyphen
        string baseSlug = Regex.Replace(cleaned, @"[\s-]+", "-").Trim('-');

        if (string.IsNullOrWhiteSpace(baseSlug))
        {
            baseSlug = "category";
        }

        var existingSlugsQuery = context.Categories
            .Where(c => (c.Slug == baseSlug || c.Slug.StartsWith(baseSlug + "-")) && c.DeletedAt == null);

        if (category.Id != Guid.Empty)
        {
            existingSlugsQuery = existingSlugsQuery.Where(c => c.Id != category.Id);
        }

        var existingSlugs = await existingSlugsQuery
            .Select(c => c.Slug)
            .ToListAsync(ct);

        var existingSet = new HashSet<string>(existingSlugs, StringComparer.OrdinalIgnoreCase);

        if (!existingSet.Contains(baseSlug))
        {
            return baseSlug;
        }

        int count = 1;
        string candidate = $"{baseSlug}-{count}";
        while (existingSet.Contains(candidate))
        {
            count++;
            candidate = $"{baseSlug}-{count}";
        }

        return candidate;
    }
}

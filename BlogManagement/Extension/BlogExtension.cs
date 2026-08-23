using System.Text.RegularExpressions;
using BlogManagement.Database;
using BlogManagement.Models;
using Microsoft.EntityFrameworkCore;

namespace BlogManagement.Extension;

public static class BlogExtension
{
    public static async Task<string> GenerateUniqueSlug(this Blog blog, AppDbContext context, CancellationToken ct = default)
    {
        string raw = (blog.Title ?? string.Empty).ToLowerInvariant().Trim();

        // Remove invalid characters (keep lowercase letters, digits, spaces, hyphens)
        string cleaned = Regex.Replace(raw, @"[^a-z0-9\s-]", string.Empty);

        // Replace consecutive whitespace/hyphens with a single hyphen
        string baseSlug = Regex.Replace(cleaned, @"[\s-]+", "-").Trim('-');

        if (string.IsNullOrWhiteSpace(baseSlug))
        {
            baseSlug = "blog-post";
        }

        var existingSlugs = await context.Blogs
            .Where(b => b.Slug == baseSlug || b.Slug.StartsWith(baseSlug + "-"))
            .Select(b => b.Slug)
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

    public static int CalculateReadingTimeMinutes(string? content)
    {
        if (string.IsNullOrWhiteSpace(content))
        {
            return 0;
        }

        // Strip HTML tags
        string plainText = Regex.Replace(content, @"<[^>]*>", " ");

        // Strip Markdown image & link syntax
        plainText = Regex.Replace(plainText, @"!\[.*?\]\(.*?\)", " ");
        plainText = Regex.Replace(plainText, @"\[.*?\]\(.*?\)", " ");

        // Split into words
        var words = plainText.Split(new[] { ' ', '\r', '\n', '\t' }, StringSplitOptions.RemoveEmptyEntries);
        int wordCount = words.Length;

        if (wordCount == 0)
        {
            return 0;
        }

        // Average reading speed: 200 words per minute
        int minutes = (int)Math.Ceiling(wordCount / 200.0);
        return Math.Max(1, minutes);
    }

    public static string GenerateSummaryFromContent(string? content, int maxLength = 160)
    {
        if (string.IsNullOrWhiteSpace(content))
        {
            return string.Empty;
        }

        // Strip HTML tags and markdown
        string plainText = Regex.Replace(content, @"<[^>]*>", " ");
        plainText = Regex.Replace(plainText, @"!\[.*?\]\(.*?\)", " ");
        plainText = Regex.Replace(plainText, @"\[.*?\]\(.*?\)", " ");
        plainText = Regex.Replace(plainText, @"\s+", " ").Trim();

        if (plainText.Length <= maxLength)
        {
            return plainText;
        }

        return plainText[..maxLength].TrimEnd() + "...";
    }

    public static List<string> ExtractUploadedImageUrls(string? content)
    {
        if (string.IsNullOrWhiteSpace(content))
        {
            return new List<string>();
        }

        var imageUrls = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

        var htmlMatches = Regex.Matches(content, @"<img[^>]+src=[""'](?<url>/uploads/[^""']+)[""']", RegexOptions.IgnoreCase);
        foreach (Match match in htmlMatches)
        {
            if (match.Groups["url"].Success)
            {
                imageUrls.Add(match.Groups["url"].Value);
            }
        }

        // Match Markdown ![alt](/uploads/...)
        var markdownMatches = Regex.Matches(content, @"!\[.*?\]\((?<url>/uploads/[^\s\)]+)\)", RegexOptions.IgnoreCase);
        foreach (Match match in markdownMatches)
        {
            if (match.Groups["url"].Success)
            {
                imageUrls.Add(match.Groups["url"].Value);
            }
        }

        return imageUrls.ToList();
    }
}

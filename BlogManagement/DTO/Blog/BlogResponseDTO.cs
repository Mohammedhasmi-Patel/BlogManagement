using BlogManagement.DTO.Category;

namespace BlogManagement.DTO.Blog;

public class BlogResponseDTO
{
    public Guid Id { get; set; }

    public string Title { get; set; } = string.Empty;

    public string Content { get; set; } = string.Empty;

    public string Slug { get; set; } = string.Empty;

    public string? Summary { get; set; }

    public string Status { get; set; } = string.Empty;

    public string? SeoJson { get; set; }

    public int ViewCount { get; set; }

    public int ReadingTimeMinutes { get; set; }

    public Guid AuthorId { get; set; }

    public string? AuthorName { get; set; }

    public string? AuthorAvatar { get; set; }

    public string? CoverImage { get; set; }

    public DateTime? PublishedAt { get; set; }

    public DateTime CreatedAt { get; set; }

    public List<CategoryResponseDTO> Categories { get; set; } = new();

    public List<BlogMediaResponseDTO> Media { get; set; } = new();
}

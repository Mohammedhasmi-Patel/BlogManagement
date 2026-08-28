namespace BlogManagement.DTO.Blog;

public class UpdateBlogRequestDTO
{
    public Guid Id { get; set; }

    public string Title { get; set; } = string.Empty;

    public string Content { get; set; } = string.Empty;

    public string? Summary { get; set; }

    public string Status { get; set; } = "draft";

    public string? SeoJson { get; set; }

    public IFormFile? CoverImage { get; set; }

    public List<Guid> CategoryIds { get; set; } = new();
}

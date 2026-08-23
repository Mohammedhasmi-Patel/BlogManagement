namespace BlogManagement.DTO.Blog;

public class UploadBlogImageResponseDTO
{
    public string FileUrl { get; set; } = string.Empty;

    public string? FullUrl { get; set; }

    public string FileName { get; set; } = string.Empty;

    public string? OriginalFileName { get; set; }

    public long FileSize { get; set; }
}

namespace BlogManagement.DTO.Bookmark;

public class BookmarkBlogListingresponseDTO
{
    public Guid Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Slug { get; set; } = string.Empty;
    public string Summary { get; set; } = string.Empty;
    public string CoverImage { get; set; } = string.Empty;
    public string AuthorName { get; set; } = string.Empty;
    public int LikeCount { get; set; } = 0;
    public bool IsLiked { get; set; } = false;
    public bool IsBookmarked { get; set; } = false;
    public DateTime CreatedAt { get; set; }
}

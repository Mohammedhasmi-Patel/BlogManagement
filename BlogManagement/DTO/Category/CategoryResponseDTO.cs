namespace BlogManagement.DTO.Category;

public class CategoryResponseDTO
{
    public Guid Id { get; set; }

    public string Name { get; set; } = string.Empty;

    public string Slug { get; set; } = string.Empty;

    public string? Description { get; set; }

    public string? Icon { get; set; }
}

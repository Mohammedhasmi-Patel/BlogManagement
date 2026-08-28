namespace BlogManagement.DTO.Category;

public class UpdateCategoryRequestDTO
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public IFormFile? Icon { get; set; }
}

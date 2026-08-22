namespace BlogManagement.DTO.Category;

public class CreateCategoryRequestDTO
{
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public IFormFile? Icon {get;set;}

}

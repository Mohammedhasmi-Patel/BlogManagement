namespace BlogManagement.DTO.Category;

public class GetCategoriesRequestDTO
{
    private int _pageSize = 10;

    public int PageNumber { get; set; } = 1;
    public int PageSize
    {
        get => _pageSize;
        set => _pageSize = value > 50 ? 50 : value;
    }
    public string? Search { get; set; }
    public string? SortBy { get; set; } = "Name";
    public string? SortOrder { get; set; } = "asc";
}

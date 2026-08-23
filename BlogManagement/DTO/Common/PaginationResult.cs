namespace BlogManagement.DTO.Common;

public class PaginationResult<T>(List<T> items, int count, int pageNumber, int pageSize)
{
    public List<T> Items { get; set; } = items;
    public int TotalCount { get; set; } = count;

    public int CurrentPage { get; set; } = pageNumber;
    public int PageSize { get; set; } = pageSize;
    public int TotalPages { get; set; } = pageSize > 0 ? (int)Math.Ceiling(count / (double)pageSize) : 0;
    public bool HasPreviousPage => CurrentPage > 1;
    public bool HasNextPage => CurrentPage < TotalPages;
}


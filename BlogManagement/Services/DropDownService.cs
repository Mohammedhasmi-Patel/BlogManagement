using BlogManagement.Database;
using BlogManagement.DTO.Common;
using BlogManagement.DTO.Dropdown;
using BlogManagement.Enum;
using BlogManagement.ServiceContracts;
using Mapster;
using Microsoft.EntityFrameworkCore;

namespace BlogManagement.Services;

public class DropDownService(AppDbContext context, IFileStorageService fileService) : IDropDownService
{
    private readonly AppDbContext _context = context;
    private readonly IFileStorageService _fileService = fileService;

    public async Task<ApiResponse<PaginationResult<AuthorDropdownResponseDTO>>> GetAuthorAsync(GetAuthorDropdownRequestDTO? requestDTO = null, CancellationToken ct = default)
    {
        requestDTO ??= new GetAuthorDropdownRequestDTO();

        string authorRole = nameof(UserRoleEnum.Author);

        var authorQuery = from user in _context.Users.AsNoTracking()
                          join userRole in _context.UserRoles on user.Id equals userRole.UserId
                          join role in _context.Roles on userRole.RoleId equals role.Id
                          where role.Name == authorRole && user.DeletedAt == null
                          select user;

        if (!string.IsNullOrWhiteSpace(requestDTO.Search))
        {
            string search = requestDTO.Search.Trim();
            authorQuery = authorQuery.Where(u =>
                u.FirstName.Contains(search) ||
                u.LastName.Contains(search) ||
                (u.UserName != null && u.UserName.Contains(search)) ||
                (u.Email != null && u.Email.Contains(search)));
        }

        authorQuery = authorQuery.OrderBy(u => u.FirstName).ThenBy(u => u.LastName);

        int totalCount = await authorQuery.CountAsync(ct);

        int skip = (requestDTO.PageNumber - 1) * requestDTO.PageSize;

        var authors = await authorQuery
            .Skip(skip)
            .Take(requestDTO.PageSize)
            .Select(u => new AuthorDropdownResponseDTO
            {
                Id = u.Id,
                FullName = (u.FirstName + " " + u.LastName).Trim()
            })
            .ToListAsync(ct);

        var paginationResult = new PaginationResult<AuthorDropdownResponseDTO>(
            authors,
            totalCount,
            requestDTO.PageNumber,
            requestDTO.PageSize
        );

        return ApiResponse<PaginationResult<AuthorDropdownResponseDTO>>.SuccessResponse(
            paginationResult,
            StatusCodes.Status200OK,
            "Authors dropdown fetched successfully."
        );
    }

    public async Task<ApiResponse<PaginationResult<CategoryDropDownResponseDTO>>> GetCategoryAsync(GetCategoryDropDownRequestDTO? requestDTO = null, CancellationToken ct = default)
    {
        requestDTO ??= new GetCategoryDropDownRequestDTO();

        var categoryQuery = _context.Categories.AsNoTracking();

        if (!string.IsNullOrWhiteSpace(requestDTO.Search))
        {
            string search = requestDTO.Search.Trim();
            categoryQuery = categoryQuery.Where(c => c.Name.Contains(search));
        }

        categoryQuery = categoryQuery.OrderBy(c => c.Name);

        int totalCount = await categoryQuery.CountAsync(ct);

        int skip = (requestDTO.PageNumber - 1) * requestDTO.PageSize;

        var categories = await categoryQuery
            .Skip(skip)
            .Take(requestDTO.PageSize)
            .Select(c => new CategoryDropDownResponseDTO()
            {
                Id = c.Id,
                Name = c.Name,
                Icon = c.Icon != null ? _fileService.GetSignedUrlAsync(c.Icon, ct) : null
            })
            .ToListAsync(ct);

        var paginationResult = new PaginationResult<CategoryDropDownResponseDTO>(
            categories,
            totalCount,
            requestDTO.PageNumber,
            requestDTO.PageSize
        );

        return ApiResponse<PaginationResult<CategoryDropDownResponseDTO>>.SuccessResponse(
            paginationResult,
            StatusCodes.Status200OK,
            "Categories dropdown fetched successfully."
        );
    }

}

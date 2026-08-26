using BlogManagement.Database;
using BlogManagement.DTO.Common;
using BlogManagement.DTO.Dropdown;
using BlogManagement.Enum;
using BlogManagement.ServiceContracts;
using Microsoft.EntityFrameworkCore;

namespace BlogManagement.Services;

public class DropDownService(AppDbContext context) : IDropDownService
{
    private readonly AppDbContext _context = context;

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
}

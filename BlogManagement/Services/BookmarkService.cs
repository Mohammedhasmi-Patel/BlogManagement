using BlogManagement.Database;
using BlogManagement.DTO.Bookmark;
using BlogManagement.DTO.Common;
using BlogManagement.Exceptions;
using BlogManagement.Models;
using BlogManagement.ServiceContracts;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace BlogManagement.Services;

public class BookmarkService(AppDbContext context, UserManager<AppUser> userManager) : IBookmarkService
{
    private readonly AppDbContext _context = context;
    private readonly UserManager<AppUser> _userManager = userManager;


    public async Task<ApiResponse<object>> CreateAsync(Guid BlogId, string userEmail, CancellationToken ct = default)
    {
        var user = await _userManager.Users.Where(x => x.Email == userEmail && x.DeletedAt == null).FirstOrDefaultAsync(ct) ?? throw new BadRequestException("Invalid token.");

        var blog = await _context.Blogs.Where(x => x.Id == BlogId).FirstOrDefaultAsync(ct) ?? throw new BadRequestException("Blog not found.");
        if (blog.AuthorId == user.Id)
        {
            throw new BadRequestException("You cannot bookmark your own blog.");
        }

        var exist = await _context.Bookmarks.Where(x => x.UserId == user.Id && x.BlogId == BlogId).FirstOrDefaultAsync(ct);
        if (exist != null)
        {
            throw new BadRequestException("You have already bookmarked this blog.");
        }

        //add bookmark
        var bookmark = new Bookmark
        {
            Id = Guid.NewGuid(),
            UserId = user.Id,
            BlogId = BlogId
        };
        await _context.Bookmarks.AddAsync(bookmark, ct);
        await _context.SaveChangesAsync(ct);

        return ApiResponse<object>.SuccessResponse(null, StatusCodes.Status201Created, "Blog bookmarked successfully.");
    }

    public async Task<ApiResponse<PaginationResult<BookmarkBlogListingresponseDTO>>> GetAllAsync(GetBookmarkRequestDTO requestDTO, string email, CancellationToken ct = default)
    {
        var user = await _userManager.Users
            .AsNoTracking()
            .Where(x => x.Email == email && x.DeletedAt == null)
            .FirstOrDefaultAsync(ct) ?? throw new NotFoundException("User not found.");

        var query = _context.Bookmarks
            .AsNoTracking()
            .Where(x => x.UserId == user.Id && x.Blog != null);

        var totalCount = await query.CountAsync(ct);

        var blogs = await query
            .OrderByDescending(x => x.CreatedAt)
            .Skip((requestDTO.PageNumber - 1) * requestDTO.PageSize)
            .Take(requestDTO.PageSize)
            .Select(x => new BookmarkBlogListingresponseDTO
            {
                Id = x.BlogId,
                Title = x.Blog.Title,
                Slug = x.Blog.Slug,
                Summary = x.Blog.Summary ?? string.Empty,
                CoverImage = x.Blog.Media
                    .Where(m => m.IsPrimary)
                    .Select(m => m.FilePath)
                    .FirstOrDefault()
                    ?? x.Blog.Media.Select(m => m.FilePath).FirstOrDefault()
                    ?? string.Empty,
                AuthorName = (x.Blog.Author.FirstName + " " + x.Blog.Author.LastName).Trim(),
                LikeCount = x.Blog.Likes.Count,
                IsLiked = x.Blog.Likes.Any(l => l.UserId == user.Id),
                IsBookmarked = true,
                CreatedAt = x.Blog.CreatedAt
            })
            .ToListAsync(ct);

        var paginationResult = new PaginationResult<BookmarkBlogListingresponseDTO>(blogs, totalCount, requestDTO.PageNumber, requestDTO.PageSize);

        return ApiResponse<PaginationResult<BookmarkBlogListingresponseDTO>>.SuccessResponse(paginationResult, StatusCodes.Status200OK, "Bookmarks retrieved successfully.");
    }

    public async Task<ApiResponse<object>> RemoveAsync(Guid blogId, string email, CancellationToken ct = default)
    {
        var user = await _userManager.Users
            .AsNoTracking()
            .Where(x => x.Email == email && x.DeletedAt == null)
            .FirstOrDefaultAsync(ct) ?? throw new NotFoundException("User not found.");

        var bookmark = await _context.Bookmarks
            .Where(x => x.UserId == user.Id && x.BlogId == blogId)
            .FirstOrDefaultAsync(ct) ?? throw new NotFoundException("Bookmark not found.");

        _context.Bookmarks.Remove(bookmark);
        await _context.SaveChangesAsync(ct);

        return ApiResponse<object>.SuccessResponse(null, StatusCodes.Status200OK, "Bookmark removed successfully.");
    }

}

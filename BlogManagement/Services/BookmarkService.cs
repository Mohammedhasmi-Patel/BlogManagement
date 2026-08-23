using BlogManagement.Database;
using BlogManagement.DTO.Common;
using BlogManagement.Exceptions;
using BlogManagement.Models;
using BlogManagement.ServiceContracts;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace BlogManagement.Services;

public class BookmarkService : IBookmarkService
{
    private readonly AppDbContext _context;
    private readonly UserManager<AppUser> _userManager;

    public BookmarkService(AppDbContext context, UserManager<AppUser> userManager)
    {
        _context = context;
        _userManager = userManager;
    }
    public async Task<ApiResponse<object>> CreateAsync(Guid BlogId, string userEmail)
    {
        // throw new NotImplementedException();
        var user = await _userManager.Users.Where(x => x.Email == userEmail && x.DeletedAt == null).FirstOrDefaultAsync() ?? throw new BadRequestException("Invalid token.");

        var blog = await _context.Blogs.Where(x => x.Id == BlogId).FirstOrDefaultAsync() ?? throw new BadRequestException("Blog not found.");
        if (blog.AuthorId == user.Id)
        {
            throw new BadRequestException("You cannot bookmark your own blog.");
        }

        var exist = await _context.Bookmarks.Where(x => x.UserId == user.Id && x.BlogId == BlogId).FirstOrDefaultAsync();
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
        await _context.Bookmarks.AddAsync(bookmark);
        await _context.SaveChangesAsync();

        return ApiResponse<object>.SuccessResponse(null, 201, "Blog bookmarked successfully.");
    }

}

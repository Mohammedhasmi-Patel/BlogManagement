using BlogManagement.Database;
using BlogManagement.DTO.Common;
using BlogManagement.DTO.Like;
using BlogManagement.Exceptions;
using BlogManagement.Models;
using BlogManagement.ServiceContracts;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace BlogManagement.Services;

public class LikeService(UserManager<AppUser> userManager, AppDbContext context) : ILikeService
{
    private readonly UserManager<AppUser> _userManager = userManager;
    private readonly AppDbContext _context = context;

    public async Task<ApiResponse<object>> CreateLikeAsync(CreateLikeRequestDTO requestDTO, string userEmail, CancellationToken ct = default)
    {
        var user = await _userManager.Users.Where(u => u.Email == userEmail && u.DeletedAt == null).FirstOrDefaultAsync(ct) ?? throw new NotFoundException("User not found!");

        var isBlogExist = await _context.Blogs.AnyAsync(b => b.Id == requestDTO.BlogId, ct);
        if (!isBlogExist)
        {
            throw new NotFoundException("Blog not found!");
        }

        var isLiked = await _context.Likes.AnyAsync(l => l.BlogId == requestDTO.BlogId && l.UserId == user.Id, ct);
        if (isLiked)
        {
            throw new ConflictException("You have already liked this blog!");
        }

        var like = new Like
        {
            Id = Guid.NewGuid(),
            BlogId = requestDTO.BlogId,
            UserId = user.Id
        };
        await _context.Likes.AddAsync(like, ct);
        await _context.SaveChangesAsync(ct);
        return ApiResponse<object>.SuccessResponse(null, StatusCodes.Status201Created, "Like created successfully!");
    }

    public async Task<ApiResponse<object>> RemoveLikeAsync(Guid blogId, string userEmail, CancellationToken ct = default)
    {
        var user = await _userManager.Users.Where(u => u.Email == userEmail && u.DeletedAt == null).FirstOrDefaultAsync(ct) ?? throw new NotFoundException("User not found!");

        var like = await _context.Likes.FirstOrDefaultAsync(l => l.BlogId == blogId && l.UserId == user.Id, ct) ?? throw new NotFoundException("Like not found for this blog!");

        _context.Likes.Remove(like);
        await _context.SaveChangesAsync(ct);
        return ApiResponse<object>.SuccessResponse(null, StatusCodes.Status200OK, "Like removed successfully!");
    }
}


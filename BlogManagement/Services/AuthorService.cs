using BlogManagement.Configurations;
using BlogManagement.Database;
using BlogManagement.DTO.Author;
using BlogManagement.DTO.Common;
using BlogManagement.Exceptions;
using BlogManagement.Extension;
using BlogManagement.Models;
using BlogManagement.ServiceContracts;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace BlogManagement.Services;

public class AuthorService(
    UserManager<AppUser> userManager,
    AppDbContext context,
    IOptions<AppSettings> options) : IAuthorService
{
    private readonly UserManager<AppUser> _userManager = userManager;
    private readonly AppDbContext _context = context;
    private readonly IOptions<AppSettings> _options = options;

    public async Task<ApiResponse<AuthorPublicProfileResponseDTO>> GetAuthorPublicProfileAsync(
        AuthorPublicProfileRequestDTO requestDTO,
        string? userEmail = null,
        CancellationToken ct = default)
    {
        var author = await _userManager.Users
            .AsNoTracking()
            .FirstOrDefaultAsync(u => u.Id == requestDTO.AuthorId && u.DeletedAt == null, ct)
            ?? throw new NotFoundException("Author not found.");

        AppUser? currentUser = null;
        if (!string.IsNullOrWhiteSpace(userEmail))
        {
            currentUser = await _userManager.Users
                .AsNoTracking()
                .FirstOrDefaultAsync(u => u.Email == userEmail && u.DeletedAt == null, ct);
        }

        int totalPosts = await _context.Blogs
            .AsNoTracking()
            .CountAsync(b => b.AuthorId == author.Id && b.Status == "published", ct);

        int totalFollowers = await _context.UserFollows
            .AsNoTracking()
            .CountAsync(f => f.AuthorId == author.Id, ct);

        bool isFollowing = currentUser != null && await _context.UserFollows
            .AsNoTracking()
            .AnyAsync(f => f.AuthorId == author.Id && f.FollowerId == currentUser.Id, ct);

        var responseData = new AuthorPublicProfileResponseDTO
        {
            Id = author.Id,
            FullName = $"{author.FirstName} {author.LastName}".Trim(),
            AvatarUrl = author.GetUserProfileUrl(_options),
            Bio = author.Bio,
            TotalPosts = totalPosts,
            TotalFollowers = totalFollowers,
            IsFollowing = isFollowing
        };

        return ApiResponse<AuthorPublicProfileResponseDTO>.SuccessResponse(
            responseData,
            StatusCodes.Status200OK,
            "Author public profile fetched successfully.");
    }
}


using BlogManagement.Database;
using BlogManagement.DTO.Common;
using BlogManagement.DTO.UserFollow;
using BlogManagement.Enum;
using BlogManagement.Exceptions;
using BlogManagement.Models;
using BlogManagement.ServiceContracts;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace BlogManagement.Services;

public class UserFollowService(UserManager<AppUser> userManager, AppDbContext context) : IUserFollowService
{
    private readonly UserManager<AppUser> _userManager = userManager;
    private readonly AppDbContext _context = context;

    public async Task<ApiResponse<FollowAuthorResponseDTO>> FollowUserAsync(CreateUserFollowRequestDTO requestDTO, string userEmail, CancellationToken ct)
    {
        var user = await _userManager.Users
            .AsNoTracking()
            .FirstOrDefaultAsync(u => u.Email == userEmail && u.DeletedAt == null, ct)
            ?? throw new NotFoundException("User not found.");

        if (user.Id == requestDTO.AuthorId)
        {
            throw new BadRequestException("You cannot follow yourself.");
        }

        var author = await _userManager.Users
            .AsNoTracking()
            .FirstOrDefaultAsync(u => u.Id == requestDTO.AuthorId && u.DeletedAt == null, ct)
            ?? throw new NotFoundException("Author not found.");

        var isAuthor = await _userManager.IsInRoleAsync(author, nameof(UserRoleEnum.Author));
        if (!isAuthor)
        {
            throw new BadRequestException("User is not an author.");
        }

        var isAlreadyFollowing = await _context.UserFollows
            .AnyAsync(x => x.FollowerId == user.Id && x.AuthorId == author.Id, ct);

        if (isAlreadyFollowing)
        {
            throw new ConflictException("You have already followed this author.");
        }

        var userFollow = new UserFollow
        {
            FollowerId = user.Id,
            AuthorId = author.Id,
        };

        await _context.UserFollows.AddAsync(userFollow, ct);
        await _context.SaveChangesAsync(ct);

        int totalFollowers = await _context.UserFollows
            .AsNoTracking()
            .CountAsync(x => x.AuthorId == author.Id, ct);

        var responseDTO = new FollowAuthorResponseDTO
        {
            AuthorId = author.Id.ToString(),
            IsFollowing = true,
            TotalFollowers = totalFollowers
        };

        return ApiResponse<FollowAuthorResponseDTO>.SuccessResponse(responseDTO, StatusCodes.Status201Created, "User followed successfully.");
    }

    public async Task<ApiResponse<FollowAuthorResponseDTO>> UnfollowUserAsync(Guid authorId, string userEmail, CancellationToken ct)
    {
        var user = await _userManager.Users
            .AsNoTracking()
            .FirstOrDefaultAsync(u => u.Email == userEmail && u.DeletedAt == null, ct)
            ?? throw new NotFoundException("User not found.");

        var userFollow = await _context.UserFollows
            .FirstOrDefaultAsync(x => x.FollowerId == user.Id && x.AuthorId == authorId, ct)
            ?? throw new NotFoundException("You are not following this author.");

        _context.UserFollows.Remove(userFollow);
        await _context.SaveChangesAsync(ct);

        int totalFollowers = await _context.UserFollows
            .AsNoTracking()
            .CountAsync(x => x.AuthorId == authorId, ct);

        var responseDTO = new FollowAuthorResponseDTO
        {
            AuthorId = authorId.ToString(),
            IsFollowing = false,
            TotalFollowers = totalFollowers
        };

        return ApiResponse<FollowAuthorResponseDTO>.SuccessResponse(responseDTO, StatusCodes.Status200OK, "User unfollowed successfully.");
    }
}


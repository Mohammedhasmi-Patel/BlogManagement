using BlogManagement.Database;
using BlogManagement.DTO.Comment;
using BlogManagement.DTO.Common;
using BlogManagement.Exceptions;
using BlogManagement.Extension;
using BlogManagement.Models;
using BlogManagement.ServiceContracts;
using Mapster;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace BlogManagement.Services;

public class CommentService(AppDbContext context, UserManager<AppUser> userManager, IFileStorageService fileService) : ICommentService
{
    private readonly AppDbContext _context = context;
    private readonly UserManager<AppUser> _userManager = userManager;
    private readonly IFileStorageService _fileService = fileService;


    public async Task<ApiResponse<CommentResponseDTO>> CreateCommentAsync(CreateCommentRequestDTO requestDTO, string userEmail, CancellationToken ct = default)
    {
        await using var transaction = await _context.Database.BeginTransactionAsync(ct);
        try
        {
            AppUser? user = await _userManager.FindByEmailAsync(userEmail) ?? throw new NotFoundException("User not found!");
            bool isBlogExist = await _context.Blogs.AnyAsync(b => b.Id == requestDTO.BlogId, ct);
            if (!isBlogExist)
            {
                throw new NotFoundException("Blog not found.");
            }

            Comment comment = new()
            {
                Id = Guid.NewGuid(),
                BlogId = requestDTO.BlogId,
                UserId = user.Id,
                Content = requestDTO.CommentText,
            };

            await _context.Comments.AddAsync(comment, ct);
            await _context.SaveChangesAsync(ct);
            await transaction.CommitAsync(ct);

            var commentResponseDTO = comment.Adapt<CommentResponseDTO>();
            commentResponseDTO.UserName = user.UserName;
            commentResponseDTO.UserAvatar = string.IsNullOrEmpty(user.Avatar) ? null : _fileService.GetSignedUrlAsync(user.Avatar);

            return ApiResponse<CommentResponseDTO>.SuccessResponse(commentResponseDTO, StatusCodes.Status201Created, "Comment created successfully!");
        }
        catch (Exception)
        {
            await transaction.RollbackAsync(ct);
            throw;
        }
    }

    public async Task<ApiResponse<CommentResponseDTO>> UpdateCommentAsync(Guid commentId, UpdateCommentRequestDTO requestDTO, string userEmail, CancellationToken ct = default)
    {
        await using var transaction = await _context.Database.BeginTransactionAsync(ct);
        try
        {
            AppUser? user = await _userManager.FindByEmailAsync(userEmail) ?? throw new NotFoundException("User not found!");
            Comment? comment = await _context.Comments.FirstOrDefaultAsync(c => c.Id == commentId, ct) ?? throw new NotFoundException("Comment not found!");
            if (user.Id != comment.UserId)
            {
                throw new ForbiddenException("You are not authorized to update this comment!");
            }

            comment.Content = requestDTO.CommentText;
            comment.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync(ct);
            await transaction.CommitAsync(ct);
            var commentResponseDTO = comment.Adapt<CommentResponseDTO>();
            commentResponseDTO.UserName = user.UserName;
            commentResponseDTO.UserAvatar = string.IsNullOrEmpty(user.Avatar) ? null : _fileService.GetSignedUrlAsync(user.Avatar);

            return ApiResponse<CommentResponseDTO>.SuccessResponse(commentResponseDTO, StatusCodes.Status200OK, "Comment updated successfully!");
        }
        catch (Exception)
        {
            await transaction.RollbackAsync(ct);
            throw;
        }
    }

    public async Task<ApiResponse<object>> DeleteCommentAsync(Guid commentId, string userEmail, CancellationToken ct = default)
    {
        await using var transaction = await _context.Database.BeginTransactionAsync(ct);
        try
        {
            AppUser? user = await _userManager.FindByEmailAsync(userEmail) ?? throw new NotFoundException("User not found!");
            Comment? comment = await _context.Comments.Include(c => c.Blog).FirstOrDefaultAsync(c => c.Id == commentId, ct) ?? throw new NotFoundException("Comment not found!");
            if (user.Id != comment.UserId)
            {
                throw new ForbiddenException("You are not authorized to delete this comment!");
            }
            comment.DeletedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync(ct);
            await transaction.CommitAsync(ct);
            return ApiResponse<object>.SuccessResponse(null, StatusCodes.Status200OK, "Comment deleted successfully!");
        }
        catch (Exception)
        {
            await transaction.RollbackAsync(ct);
            throw;
        }
    }
}

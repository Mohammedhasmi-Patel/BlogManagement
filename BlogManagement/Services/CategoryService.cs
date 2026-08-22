using BlogManagement.Database;
using BlogManagement.DTO.Category;
using BlogManagement.DTO.Common;
using BlogManagement.Exceptions;
using BlogManagement.Extension;
using BlogManagement.Models;
using BlogManagement.ServiceContracts;
using Mapster;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace BlogManagement.Services;

public class CategoryService : ICategoryService
{
    private readonly AppDbContext _context;
    private readonly UserManager<AppUser> _userManager;
    private readonly  IFileStorageService _storageService;


    public CategoryService(AppDbContext context,UserManager<AppUser> userManager,IFileStorageService storageService)
    {
        _context = context;
        _userManager = userManager;
        _storageService = storageService;
    }
    public async Task<ApiResponse<CategoryResponseDTO>> CreateAsync(CreateCategoryRequestDTO requestDTO, string email, CancellationToken ct)
    {
        await using var transaction = await _context.Database.BeginTransactionAsync(ct);
        string? fileUrl = null;

        try
        {
            AppUser? appUser = await _userManager.FindByEmailAsync(email);
            if (appUser is null)
            {
                throw new UnauthorizedException("Unauthorized user.");
            }

            string trimmedName = (requestDTO.Name ?? string.Empty).Trim();
            string normalizedName = trimmedName.ToLower();

            var categoryExists = await _context.Categories
                .Where(c => c.CreatedBy == appUser.Id)
                .AnyAsync(c => c.Name.ToLower() == normalizedName, ct);

            if (categoryExists)
            {
                throw new ConflictException($"Category '{trimmedName}' already exists.");
            }

            if (requestDTO.Icon != null && requestDTO.Icon.Length > 0)
            {
                var fileResponse = await _storageService.UploadAsync(requestDTO.Icon, "categories", isFileRequired: false, ct);
                fileUrl = fileResponse?.FileUrl;
            }

            Category category = requestDTO.Adapt<Category>();
            category.Icon = fileUrl;
            category.CreatedBy = appUser.Id;
            category.Slug = await category.GenerateUniqueSlug(_context, ct);

            await _context.Categories.AddAsync(category, ct);
            await _context.SaveChangesAsync(ct);
            await transaction.CommitAsync(ct);

            var responseData = category.Adapt<CategoryResponseDTO>();
            return ApiResponse<CategoryResponseDTO>.SuccessResponse(responseData, 201, "Category created successfully.");
        }
        catch (Exception)
        {
            await transaction.RollbackAsync(ct);

            if (!string.IsNullOrWhiteSpace(fileUrl))
            {
                await _storageService.DeleteAsync(fileUrl, CancellationToken.None);
            }

            throw;
        }
    }
}

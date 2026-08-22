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
    public async Task<ApiResponse<CategoryResponseDTO>> CreateAsync(CreateCategoryRequestDTO requestDTO,string email,CancellationToken ct)
    {
        // throw new NotImplementedException();
        await using var transaction = await _context.Database.BeginTransactionAsync(ct);

        try
        {
            AppUser? appUser = await _userManager.FindByEmailAsync(email);
             string? fileUrl = null;
            if (appUser is null)
            {
                throw new UnauthorizedAccessException("Unauthorized user.");
            }
            var categoryNameFromDb = await _context.Categories.Where(c => c.CreatedBy == appUser.Id)
                                                                    .AnyAsync(c => string.Equals(c.Name, requestDTO.Name, StringComparison.OrdinalIgnoreCase), ct);
            if (categoryNameFromDb)
            {
                throw new BadRequestException("Category name exist");
            }

            if (requestDTO.Icon?.Length > 0)
            {
                var fileresponse = await _storageService.UploadAsync(requestDTO.Icon, "categories", true, ct);
                fileUrl = fileresponse?.FileUrl;
            }

            Category category = requestDTO.Adapt<Category>();
            category.Slug = await category.GenerateUniqueSlug(_context);
            category.Icon = fileUrl;
            category.CreatedBy = appUser.Id;
            var result = await _context.Categories.AddAsync(category, ct) ?? throw new BadRequestException("Something went wrong while adding the category.");
            await _context.SaveChangesAsync(ct);
            await transaction.CommitAsync(ct);

            var responseData = category.Adapt<CategoryResponseDTO>();
            return ApiResponse<CategoryResponseDTO>.SuccessResponse(responseData,201,"Category created successfully.");
        }
        catch (Exception ex)
        {
            await transaction.RollbackAsync(ct);
            throw;
        }
    }
}

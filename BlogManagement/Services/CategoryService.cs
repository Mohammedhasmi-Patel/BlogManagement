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

public class CategoryService(AppDbContext context, UserManager<AppUser> userManager, IFileStorageService storageService) : ICategoryService
{
    private readonly AppDbContext _context = context;
    private readonly UserManager<AppUser> _userManager = userManager;
    private readonly IFileStorageService _storageService = storageService;

    public async Task<ApiResponse<CategoryResponseDTO>> CreateAsync(CreateCategoryRequestDTO requestDTO, string email, CancellationToken ct)
    {
        AppUser? appUser = await _userManager.FindByEmailAsync(email) 
            ?? throw new UnauthorizedException("Unauthorized user.");

        var categoryNameExists = await _context.Categories.AnyAsync(
            c => c.CreatedBy == appUser.Id && c.DeletedAt == null && c.Name.ToLower() == requestDTO.Name.Trim().ToLower(), 
            ct);

        if (categoryNameExists)
        {
            throw new ConflictException("Category name already exists.");
        }

        string? fileUrl = null;
        if (requestDTO.Icon?.Length > 0)
        {
            var fileresponse = await _storageService.UploadAsync(requestDTO.Icon, "categories", true, ct);
            fileUrl = fileresponse?.FileUrl;
        }

        Category category = requestDTO.Adapt<Category>();
        category.Name = requestDTO.Name.Trim();
        category.Description = requestDTO.Description?.Trim();
        category.Slug = await category.GenerateUniqueSlug(_context, ct: ct);
        category.Icon = fileUrl;
        category.CreatedBy = appUser.Id;

        await using var transaction = await _context.Database.BeginTransactionAsync(ct);

        try
        {
            await _context.Categories.AddAsync(category, ct);
            await _context.SaveChangesAsync(ct);
            await transaction.CommitAsync(ct);

            var responseData = category.Adapt<CategoryResponseDTO>();
            responseData.Icon = category.Icon != null ? _storageService.GetSignedUrlAsync(category.Icon, ct) : null;

            return ApiResponse<CategoryResponseDTO>.SuccessResponse(responseData, 201, "Category created successfully.");
        }
        catch
        {
            await transaction.RollbackAsync(ct);
            if (!string.IsNullOrEmpty(fileUrl))
            {
                await _storageService.DeleteAsync(fileUrl, ct);
            }
            throw;
        }
    }

    public async Task<ApiResponse<PaginationResult<CategoryResponseDTO>>> GetAllAsync(GetCategoriesRequestDTO requestDTO, string email, CancellationToken ct)
    {
        AppUser? appUser = await _userManager.FindByEmailAsync(email) 
            ?? throw new UnauthorizedException("Unauthorized user.");

        var categoryQuery = _context.Categories
            .AsNoTracking()
            .Where(c => c.DeletedAt == null && c.CreatedBy == appUser.Id);

        if (!string.IsNullOrWhiteSpace(requestDTO.Search))
        {
            string search = requestDTO.Search.Trim();
            categoryQuery = categoryQuery.Where(c =>
                c.Name.Contains(search) ||
                c.Slug.Contains(search) ||
                (c.Description != null && c.Description.Contains(search)));
        }

        categoryQuery = (requestDTO.SortBy?.Trim().ToLower(), requestDTO.SortOrder?.Trim().ToLower()) switch
        {
            ("name", "desc") => categoryQuery.OrderByDescending(c => c.Name),
            ("name", _) => categoryQuery.OrderBy(c => c.Name),
            ("slug", "desc") => categoryQuery.OrderByDescending(c => c.Slug),
            ("slug", _) => categoryQuery.OrderBy(c => c.Slug),
            ("description", "desc") => categoryQuery.OrderByDescending(c => c.Description),
            ("description", _) => categoryQuery.OrderBy(c => c.Description),
            (_, "asc") => categoryQuery.OrderBy(c => c.CreatedAt),
            _ => categoryQuery.OrderByDescending(c => c.CreatedAt),
        };

        int totalCount = await categoryQuery.CountAsync(ct);

        int skip = (requestDTO.PageNumber - 1) * requestDTO.PageSize;

        List<CategoryResponseDTO> categoriesListingDTO = await categoryQuery
            .Skip(skip)
            .Take(requestDTO.PageSize)
            .ProjectToType<CategoryResponseDTO>()
            .ToListAsync(ct);

        foreach (var category in categoriesListingDTO)
        {
            if (category.Icon != null)
            {
                var fileResponse = _storageService.GetSignedUrlAsync(category.Icon, ct);
                category.Icon = fileResponse;
            }
        }

        var responseDTO = new PaginationResult<CategoryResponseDTO>(categoriesListingDTO, totalCount, requestDTO.PageNumber, requestDTO.PageSize);
        return ApiResponse<PaginationResult<CategoryResponseDTO>>.SuccessResponse(responseDTO, 200, "Categories list fetched successfully.");
    }

    public async Task<ApiResponse<CategoryResponseDTO>> GetByIdAsync(Guid id, string email, CancellationToken ct)
    {
        AppUser? appUser = await _userManager.FindByEmailAsync(email) 
            ?? throw new UnauthorizedException("Unauthorized user.");

        Category category = await _context.Categories
            .AsNoTracking()
            .FirstOrDefaultAsync(c => c.Id == id && c.CreatedBy == appUser.Id && c.DeletedAt == null, ct) 
            ?? throw new NotFoundException("Category not found.");

        var fileUrl = category.Icon != null ? _storageService.GetSignedUrlAsync(category.Icon, ct) : null;
        var responseData = category.Adapt<CategoryResponseDTO>();
        responseData.Icon = fileUrl;

        return ApiResponse<CategoryResponseDTO>.SuccessResponse(responseData, 200, "Category fetched successfully.");
    }

    public async Task<ApiResponse<CategoryResponseDTO>> UpdateAsync(UpdateCategoryRequestDTO requestDTO, string email, CancellationToken ct)
    {
        AppUser? appUser = await _userManager.FindByEmailAsync(email) 
            ?? throw new UnauthorizedException("Unauthorized user.");

        Category category = await _context.Categories
            .FirstOrDefaultAsync(c => c.Id == requestDTO.Id && c.CreatedBy == appUser.Id && c.DeletedAt == null, ct) 
            ?? throw new NotFoundException("Category not found.");

        bool existingCategory = await _context.Categories.AnyAsync(
            c => c.CreatedBy == appUser.Id && 
                 c.DeletedAt == null && 
                 c.Id != requestDTO.Id && 
                 c.Name.ToLower() == requestDTO.Name.Trim().ToLower(), 
            ct);

        if (existingCategory)
        {
            throw new ConflictException("Category name already exists.");
        }

        string? oldIcon = category.Icon;
        string? newIconUrl = null;
        bool iconUpdated = false;

        if (requestDTO.Icon?.Length > 0)
        {
            var fileResponse = await _storageService.UploadAsync(requestDTO.Icon, "categories", true, ct);
            newIconUrl = fileResponse?.FileUrl;
            iconUpdated = true;
        }

        category.Name = requestDTO.Name.Trim();
        category.Slug = await category.GenerateUniqueSlug(_context, ct: ct);
        category.Description = requestDTO.Description?.Trim();
        if (iconUpdated)
        {
            category.Icon = newIconUrl;
        }

        await using var transaction = await _context.Database.BeginTransactionAsync(ct);

        try
        {
            await _context.SaveChangesAsync(ct);
            await transaction.CommitAsync(ct);

            if (iconUpdated && !string.IsNullOrEmpty(oldIcon))
            {
                await _storageService.DeleteAsync(oldIcon, ct);
            }

            var responseData = category.Adapt<CategoryResponseDTO>();
            responseData.Icon = category.Icon != null ? _storageService.GetSignedUrlAsync(category.Icon, ct) : null;

            return ApiResponse<CategoryResponseDTO>.SuccessResponse(responseData, 200, "Category updated successfully.");
        }
        catch
        {
            await transaction.RollbackAsync(ct);
            if (iconUpdated && !string.IsNullOrEmpty(newIconUrl))
            {
                await _storageService.DeleteAsync(newIconUrl, ct);
            }
            throw;
        }
    }
}


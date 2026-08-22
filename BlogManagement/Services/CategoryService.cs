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
    private readonly  IFileStorageService _storageService = storageService;


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
            // await _storageService.DeleteAsync()
            await transaction.RollbackAsync(ct);
            throw;
        }
    }

    public async Task<ApiResponse<PaginationResult<CategoryResponseDTO>>> GetAllAsync(GetCategoriesRequestDTO requestDTO,string userEmail, CancellationToken ct)
    {
        // throw new NotImplementedException();
        var categoryQuery = _context.Categories.AsQueryable();
        string search = requestDTO.Search.ToLower();
        string sortOrder = requestDTO.SortOrder;
        if (!string.IsNullOrEmpty(search))
        {
            categoryQuery = categoryQuery.Where(c => c.Name.Contains(search) || c.Slug.Contains(search) || c.Description.Contains(search));
        }

        var SortBy = requestDTO.SortBy switch
        {
            "Name" => sortOrder == "asc" ? categoryQuery = categoryQuery.OrderBy(c => c.Name) : categoryQuery = categoryQuery.OrderByDescending(c => c.Name),
            "Slug" => sortOrder == "asc" ? categoryQuery = categoryQuery.OrderBy(c => c.Slug) : categoryQuery = categoryQuery.OrderByDescending(c => c.Slug),
            "Description" => sortOrder == "asc" ? categoryQuery = categoryQuery.OrderBy(c => c.Description) : categoryQuery = categoryQuery.OrderByDescending(c => c.Description),
            _ => sortOrder == "asc" ? categoryQuery = categoryQuery.OrderBy(c => c.CreatedAt) : categoryQuery = categoryQuery.OrderByDescending(c => c.CreatedAt),
        };

        int totalCount = await categoryQuery.CountAsync(ct);

        List<Category> categories = await categoryQuery
                                        .Skip(requestDTO.PageNumber * requestDTO.PageSize)
                                        .Take(requestDTO.PageSize)
                                        .ToListAsync(cancellationToken: ct);

        List<CategoryResponseDTO> categoriesListingDTO = categories.ConvertAll(c => c.Adapt<CategoryResponseDTO>());
        var responseDTO = new PaginationResult<CategoryResponseDTO>(categoriesListingDTO, totalCount, requestDTO.PageNumber, requestDTO.PageSize);
        return ApiResponse<PaginationResult<CategoryResponseDTO>>.SuccessResponse(responseDTO, 200, "Categories list fetched successfully.");
    }

}

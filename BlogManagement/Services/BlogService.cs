using BlogManagement.Configurations;
using BlogManagement.Database;
using BlogManagement.DTO.Blog;
using BlogManagement.DTO.Category;
using BlogManagement.DTO.Common;
using BlogManagement.Exceptions;
using BlogManagement.Extension;
using BlogManagement.Models;
using BlogManagement.ServiceContracts;
using Mapster;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace BlogManagement.Services;

public class BlogService(
    AppDbContext context,
    UserManager<AppUser> userManager,
    IFileStorageService fileStorageService,
    IOptions<AppSettings> appSettings) : IBlogService
{
    private readonly AppDbContext _context = context;
    private readonly UserManager<AppUser> _userManager = userManager;
    private readonly IFileStorageService _fileStorageService = fileStorageService;
    private readonly IOptions<AppSettings> _appSettings = appSettings;

    public async Task<ApiResponse<UploadBlogImageResponseDTO>> UploadContentImageAsync(IFormFile? file, CancellationToken ct = default)
    {
        if (file is null || file.Length == 0)
        {
            throw new BadRequestException("Image file is required.");
        }

        var uploadResult = await _fileStorageService.UploadAsync(file, "blogs", isFileRequired: true, ct)
            ?? throw new BadRequestException("Failed to upload image.");

        var responseData = uploadResult.Adapt<UploadBlogImageResponseDTO>();
        responseData.FullUrl = _fileStorageService.GetSignedUrlAsync(uploadResult.FileUrl, ct);

        return ApiResponse<UploadBlogImageResponseDTO>.SuccessResponse(responseData, 200, "Image uploaded successfully.");
    }

    public async Task<ApiResponse<BlogResponseDTO>> CreateAsync(CreateBlogRequestDTO requestDTO, string authorEmail, CancellationToken ct = default)
    {
        var author = await _userManager.FindByEmailAsync(authorEmail)
            ?? throw new UnauthorizedException("Unauthorized user.");

        // 2. Check duplicate title by the same author
        var isDuplicateTitle = await _context.Blogs.AnyAsync(b => b.AuthorId == author.Id && b.Title.ToLower() == requestDTO.Title.Trim().ToLower(), ct);
        if (isDuplicateTitle)
        {
            throw new ConflictException("You already have a blog with this title.");
        }

        List<Category> selectedCategories = [];
        if (requestDTO.CategoryIds.Count > 0)
        {
            var distinctCategoryIds = requestDTO.CategoryIds.Distinct().ToList();
            selectedCategories = await _context.Categories
                .Where(c => distinctCategoryIds.Contains(c.Id) && c.DeletedAt == null)
                .ToListAsync(ct);

            if (selectedCategories.Count != distinctCategoryIds.Count)
            {
                throw new BadRequestException("One or more selected categories do not exist.");
            }
        }

        Blog blog = requestDTO.Adapt<Blog>();
        blog.AuthorId = author.Id;
        blog.ReadingTimeMinutes = BlogExtension.CalculateReadingTimeMinutes(requestDTO.Content);
        blog.Summary = !string.IsNullOrWhiteSpace(requestDTO.Summary)
            ? requestDTO.Summary.Trim()
            : BlogExtension.GenerateSummaryFromContent(requestDTO.Content);
        blog.Status = requestDTO.Status.Trim().ToLower();
        blog.PublishedAt = blog.Status == "published" ? DateTime.UtcNow : null;
        blog.Slug = await blog.GenerateUniqueSlug(_context, ct);

        string? uploadedCoverFilePath = null;
        await using var transaction = await _context.Database.BeginTransactionAsync(ct);

        try
        {
            await _context.Blogs.AddAsync(blog, ct);
            await _context.SaveChangesAsync(ct);

            List<BlogHasMedia> mediaList = [];

            if (requestDTO.CoverImage != null)
            {
                var coverUpload = await _fileStorageService.UploadAsync(requestDTO.CoverImage, "blogs", isFileRequired: false, ct);
                if (coverUpload != null)
                {
                    uploadedCoverFilePath = coverUpload.FilePath;
                    var coverMedia = coverUpload.Adapt<BlogHasMedia>();
                    coverMedia.BlogId = blog.Id;
                    coverMedia.IsPrimary = true;
                    coverMedia.DisplayOrder = 0;

                    await _context.BlogHasMedia.AddAsync(coverMedia, ct);
                    mediaList.Add(coverMedia);
                }
            }

            var contentImageUrls = BlogExtension.ExtractUploadedImageUrls(blog.Content);
            int displayOrder = 1;
            foreach (var imgUrl in contentImageUrls)
            {
                var contentMedia = new BlogHasMedia
                {
                    BlogId = blog.Id,
                    FilePath = imgUrl,
                    FileName = Path.GetFileName(imgUrl),
                    MimeType = Path.GetExtension(imgUrl),
                    IsPrimary = false,
                    DisplayOrder = displayOrder++
                };
                await _context.BlogHasMedia.AddAsync(contentMedia, ct);
                mediaList.Add(contentMedia);
            }

            // Categories Association
            if (selectedCategories.Count > 0)
            {
                var blogCategories = selectedCategories.ConvertAll(c => new BlogHasCategory
                {
                    BlogId = blog.Id,
                    CategoryId = c.Id
                });

                await _context.BlogCategories.AddRangeAsync(blogCategories, ct);
            }

            await _context.SaveChangesAsync(ct);
            await transaction.CommitAsync(ct);

            var responseDTO = blog.Adapt<BlogResponseDTO>();
            responseDTO.AuthorName = $"{author.FirstName} {author.LastName}".Trim();
            responseDTO.AuthorAvatar = author.GetUserProfileUrl(_appSettings);

            responseDTO.Categories = selectedCategories.Adapt<List<CategoryResponseDTO>>();
            responseDTO.Categories.ForEach(c => c.Icon = _fileStorageService.GetSignedUrlAsync(c.Icon, ct));

            responseDTO.Media = mediaList.Adapt<List<BlogMediaResponseDTO>>();
            responseDTO.Media.ForEach(m => m.FileUrl = _fileStorageService.GetSignedUrlAsync(m.FilePath, ct));

            var primaryMedia = mediaList.FirstOrDefault(m => m.IsPrimary);
            responseDTO.CoverImage = primaryMedia != null ? _fileStorageService.GetSignedUrlAsync(primaryMedia.FilePath, ct) : null;

            return ApiResponse<BlogResponseDTO>.SuccessResponse(responseDTO, StatusCodes.Status201Created, "Blog created successfully.");
        }
        catch
        {
            await transaction.RollbackAsync(ct);

            if (!string.IsNullOrEmpty(uploadedCoverFilePath))
            {
                await _fileStorageService.DeleteAsync(uploadedCoverFilePath, ct);
            }

            throw;
        }
    }
}

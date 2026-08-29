using BlogManagement.Configurations;
using BlogManagement.DTO.FileStorage;
using BlogManagement.Exceptions;
using BlogManagement.ServiceContracts;
using Microsoft.Extensions.Options;

namespace BlogManagement.Services;

public class FileStorageService(IWebHostEnvironment env, IOptions<FileUploadSettings> options, IOptions<AppSettings> _options) : IFileStorageService
{

    private readonly IWebHostEnvironment _env = env;
    private readonly FileUploadSettings _fileSetting = options.Value;
    private readonly AppSettings _setting = _options.Value;

    public async Task<FileUploadResultResponseDTO?> UploadAsync(IFormFile? file, string folder, bool isFileRequired = false, CancellationToken cancellationToken = default)
    {
        string[] allowedExtensions = _fileSetting.AllowedFileExtensions;

        // validation for file 
        if (file is null || file.Length == 0)
        {
            if (isFileRequired)
            {
                throw new BadRequestException("File is required.");
            }
            return null;
        }

        if (file.Length > _fileSetting.MaxFileSize)
        {
            throw new BadRequestException($"File size must be less than {_fileSetting.MaxFileSize}");
        }

        if (!allowedExtensions.Contains(Path.GetExtension(file.FileName).ToLower()))
        {
            throw new BadRequestException($"File type must be {string.Join(",", allowedExtensions)}.");
        }

        string webRoot = !string.IsNullOrWhiteSpace(_env.WebRootPath) ? _env.WebRootPath : Path.Combine(_env.ContentRootPath, "wwwroot");
        string uploadFolder = Path.Combine(webRoot, "uploads", folder);
        string uniqueFilename = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);

        if (!Directory.Exists(uploadFolder))
        {
            Directory.CreateDirectory(uploadFolder);
        }

        string filePath = Path.Combine(uploadFolder, uniqueFilename);
        await using (var fileStream = new FileStream(filePath, FileMode.Create))
        {
            await file.CopyToAsync(fileStream, cancellationToken);
        }

        return new FileUploadResultResponseDTO()
        {
            OriginalFileName = file.FileName,
            FileName = uniqueFilename,
            FilePath = filePath,
            FileUrl = $"/uploads/{folder}/{uniqueFilename}",
            Extension = Path.GetExtension(file.FileName),
            FileSize = file.Length,
        };
    }
    public Task DeleteAsync(string filePath, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(filePath))
        {
            return Task.CompletedTask;
        }

        try
        {
            string fullPath = filePath;
            if (!Path.IsPathRooted(filePath) || filePath.StartsWith('/') || filePath.StartsWith('\\'))
            {
                var relativePath = filePath.TrimStart('/', '\\').Replace('/', Path.DirectorySeparatorChar);
                string webRoot = !string.IsNullOrWhiteSpace(_env.WebRootPath) ? _env.WebRootPath : Path.Combine(_env.ContentRootPath, "wwwroot");
                fullPath = Path.Combine(webRoot, relativePath);
            }

            if (File.Exists(fullPath))
            {
                File.Delete(fullPath);
            }
        }
        catch
        {
            // Ignore failure during cleanup to avoid suppressing primary exceptions
        }

        return Task.CompletedTask;
    }

    public string? GetSignedUrlAsync(string? fileUrl, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(fileUrl))
        {
            return null;
        }

        if (fileUrl.StartsWith("http://", StringComparison.OrdinalIgnoreCase) ||
            fileUrl.StartsWith("https://", StringComparison.OrdinalIgnoreCase))
        {
            return fileUrl;
        }

        string baseUrl = _setting.BaseUrl?.TrimEnd('/') ?? string.Empty;
        var path = fileUrl.StartsWith('/') ? fileUrl : $"/{fileUrl}";
        return $"{baseUrl}{path}";
    }

}

using BlogManagement.DTO.FileStorage;

namespace BlogManagement.ServiceContracts;

public interface IFileStorageService
{
    Task<FileUploadResultResponseDTO?> UploadAsync(IFormFile? file, string folder, bool isFileRequired = false, CancellationToken cancellationToken = default);

    Task DeleteAsync(string filePath, CancellationToken cancellationToken = default);

}

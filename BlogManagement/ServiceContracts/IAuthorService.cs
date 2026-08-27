using BlogManagement.DTO.Author;
using BlogManagement.DTO.Common;

namespace BlogManagement.ServiceContracts;

public interface IAuthorService
{
    Task<ApiResponse<AuthorPublicProfileResponseDTO>> GetAuthorPublicProfileAsync(AuthorPublicProfileRequestDTO requestDTO, string? userEmail = null, CancellationToken ct = default);
}

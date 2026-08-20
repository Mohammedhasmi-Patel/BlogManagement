using BlogManagement.DTO.Auth;
using BlogManagement.DTO.Common;

namespace BlogManagement.ServiceContracts;

public interface IAuthService
{
    public Task<ApiResponse<RegisterUserResponseDTO>> RegisterUserAsync(RegisterUserRequestDTO registerUserRequestDTO);
    
}

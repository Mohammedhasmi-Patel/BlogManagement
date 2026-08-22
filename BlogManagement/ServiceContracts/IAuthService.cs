using BlogManagement.DTO.Auth;
using BlogManagement.DTO.Common;

namespace BlogManagement.ServiceContracts;

public interface IAuthService
{
    Task<ApiResponse<RegisterUserResponseDTO>> RegisterUserAsync(RegisterUserRequestDTO registerUserRequestDTO);
    Task<ApiResponse<LoginResponseDTO>> LoginUserAsync(LoginRequestDTO loginRequestDTO);
}

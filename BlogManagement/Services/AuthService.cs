using BlogManagement.Configurations;
using BlogManagement.Database;
using BlogManagement.DTO.Auth;
using BlogManagement.DTO.Common;
using BlogManagement.Enum;
using BlogManagement.Exceptions;
using BlogManagement.Extension;
using BlogManagement.Models;
using BlogManagement.ServiceContracts;
using Mapster;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;


namespace BlogManagement.Services;

public class AuthService(AppDbContext context,UserManager<AppUser> userManager,IFileStorageService fileService,ITokenService tokenService,IOptions<AppSettings> options) : IAuthService
{
    private readonly UserManager<AppUser> _userManager = userManager;
    private readonly IFileStorageService _fileService = fileService;
    private readonly ITokenService _tokenService = tokenService;
    private readonly AppDbContext _context = context;
    private readonly IOptions<AppSettings> _options = options;

    public async Task<ApiResponse<RegisterUserResponseDTO>> RegisterUserAsync(RegisterUserRequestDTO requestDTO, CancellationToken ct = default)
    {
        var existingUser = await _userManager.FindByEmailAsync(requestDTO.Email);
        if (existingUser != null)
        {
            throw new ConflictException("User with this email already exists.");
        }

        if (!System.Enum.TryParse<UserRoleEnum>(requestDTO.Role, ignoreCase: true, out var roleEnum) || roleEnum == UserRoleEnum.Admin)
        {
            throw new BadRequestException("Invalid user role.");
        }

        string role = roleEnum.ToString();
        string? uploadedFilePath = null;

        await using var transaction = await _context.Database.BeginTransactionAsync(ct);
        try
        {
            var user = requestDTO.Adapt<AppUser>();
            user.UserName = requestDTO.Email;

            if (requestDTO.Avatar != null)
            {
                var uploadResult = await _fileService.UploadAsync(requestDTO.Avatar, "users", isFileRequired: false, cancellationToken: ct);
                if (uploadResult != null)
                {
                    user.Avatar = uploadResult.FileUrl;
                    uploadedFilePath = uploadResult.FilePath;
                }
            }

            var result = await _userManager.CreateAsync(user, requestDTO.Password);
            if (!result.Succeeded)
            {
                string firstError = result.Errors.Select(e => e.Description).FirstOrDefault() ?? "Failed to create user.";
                throw new BadRequestException(firstError);
            }

            var roleResult = await _userManager.AddToRoleAsync(user, role);
            if (!roleResult.Succeeded)
            {
                string firstMessage = roleResult.Errors.Select(e => e.Description)?.FirstOrDefault() ?? "Failed to assign role to user.";
                throw new BadRequestException(firstMessage);
            }

            await transaction.CommitAsync(ct);

            var responseData = user.Adapt<RegisterUserResponseDTO>();
            responseData.Token = await _tokenService.GenerateJwtTokenAsync(user);
            responseData.Avatar = user.GetUserProfileUrl(_options);
            var userRole = (await _userManager.GetRolesAsync(user)).FirstOrDefault();
            responseData.Role = userRole;

            return ApiResponse<RegisterUserResponseDTO>.SuccessResponse(responseData, StatusCodes.Status201Created, "User registered successfully.");
        }
        catch
        {
            await transaction.RollbackAsync(ct);

            if (!string.IsNullOrEmpty(uploadedFilePath))
            {
                await _fileService.DeleteAsync(uploadedFilePath, ct);
            }

            throw;
        }
    }

    public async Task<ApiResponse<LoginResponseDTO>> LoginUserAsync(LoginRequestDTO loginRequestDTO, CancellationToken ct = default)
    {
        var user = await _userManager.FindByEmailAsync(loginRequestDTO.Email);
        if (user == null || user.DeletedAt != null)
        {
            throw new UnauthorizedException("Invalid email or password.");
        }

        var isPasswordValid = await _userManager.CheckPasswordAsync(user, loginRequestDTO.Password);
        if (!isPasswordValid)
        {
            throw new UnauthorizedException("Invalid email or password.");
        }

        var responseData = user.Adapt<LoginResponseDTO>();
        responseData.Token = await _tokenService.GenerateJwtTokenAsync(user);
        responseData.Avatar = user.GetUserProfileUrl(_options);
        var userRole = (await _userManager.GetRolesAsync(user)).FirstOrDefault();
        responseData.Role = userRole;

        return ApiResponse<LoginResponseDTO>.SuccessResponse(responseData, StatusCodes.Status200OK, "User logged in successfully.");
    }
}

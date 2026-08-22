using BlogManagement.Models;

namespace BlogManagement.ServiceContracts;

public interface ITokenService
{
    public  Task<string> GenerateJwtTokenAsync(AppUser user);
}

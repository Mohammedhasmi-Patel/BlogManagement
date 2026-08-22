using BlogManagement.Models;
using BlogManagement.ServiceContracts;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using BlogManagement.Extension;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using BlogManagement.Configurations;
using Microsoft.Extensions.Options;


namespace BlogManagement.Services
{
    public class TokenService : ITokenService
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly JwtConfiguration _jwtConfig;


        public TokenService(UserManager<AppUser> userManager, IOptions<JwtConfiguration> options)
        {
            _userManager = userManager;
            _jwtConfig = options.Value;
        }
        public async Task<string> GenerateJwtTokenAsync(AppUser user)
        {
            string role = await user.GetUserRole(_userManager);
            Claim[] claims = new Claim[]
            {
                new Claim(JwtRegisteredClaimNames.Sub,user.Id.ToString()),
                new Claim(JwtRegisteredClaimNames.Jti,Guid.NewGuid().ToString()),
                new Claim(JwtRegisteredClaimNames.Email,user.Email!),
                new Claim(ClaimTypes.Name,user.UserName),
                new Claim(ClaimTypes.Role,role)
            };

            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtConfig.SecretKey));
            var signingCredentials = new SigningCredentials(securityKey,SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
              issuer: _jwtConfig.Issuer,
              audience: _jwtConfig.Audience,
              claims: claims,
              expires : DateTime.UtcNow.AddMinutes(_jwtConfig.JwtExpireInMinutes),
              signingCredentials: signingCredentials
             );

            var tokenHandler = new JwtSecurityTokenHandler();
            return tokenHandler.WriteToken(token);
        }
    }
}

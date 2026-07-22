using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using SpookyTattoos.Application.Interfaces.External;
using SpookyTattoos.Domain.Entities;

namespace SpookyTattoos.Application.Services;

public class TokenService : ITokenService
{
    private readonly IConfiguration _configuration;

    public TokenService(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public string GenerateToken(Admin admin)
    {
        var tokenHandler = new JwtSecurityTokenHandler();
        
        var secretKeyValue = _configuration["JWT_SECRET_KEY"];
        
        if (string.IsNullOrEmpty(secretKeyValue))
        {
            throw new InvalidOperationException("JWT_SECRET_KEY not found in .env file");
        }

        var key = Encoding.ASCII.GetBytes(secretKeyValue);

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, admin.Id.ToString()),
                new Claim(JwtRegisteredClaimNames.UniqueName, admin.Username),
                new Claim(ClaimTypes.Role, "Admin"), 
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()) 
            }),
            Expires = DateTime.UtcNow.AddHours(5), 
            SigningCredentials = new SigningCredentials(
                new SymmetricSecurityKey(key), 
                SecurityAlgorithms.HmacSha256Signature)
        };

        var token = tokenHandler.CreateToken(tokenDescriptor);
        return tokenHandler.WriteToken(token);
    }
}
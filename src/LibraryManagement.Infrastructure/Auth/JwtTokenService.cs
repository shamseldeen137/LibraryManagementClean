using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using LibraryManagement.Application.Auth;
using LibraryManagement.Infrastructure.Options;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace LibraryManagement.Infrastructure.Auth;

public sealed class JwtTokenService : ITokenService
{
    private readonly JwtOptions _options;

    public JwtTokenService(IOptions<JwtOptions> options)
    {
        _options = options.Value;
    }

    public AuthResponse CreateToken(ApplicationUser user)
    {
        var expires = DateTime.UtcNow.AddMinutes(_options.ExpirationMinutes);
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_options.SigningKey));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
        var claims = new[]
        {
            new Claim(JwtRegisteredClaimNames.Sub, user.Id),
            new Claim(JwtRegisteredClaimNames.Email, user.Email ?? string.Empty),
            new Claim(ClaimTypes.NameIdentifier, user.Id),
            new Claim(ClaimTypes.Name, user.DisplayName)
        };

        var token = new JwtSecurityToken(_options.Issuer, _options.Audience, claims, expires: expires, signingCredentials: credentials);
        return new AuthResponse(new JwtSecurityTokenHandler().WriteToken(token), expires, user.Id, user.Email ?? string.Empty, user.DisplayName);
    }
}

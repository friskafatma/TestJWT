using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using JWTApi.Models;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace JWTApi.Services;

public interface ITokenService
{
    string GenerateAccess(User user, out DateTime expires);
    RefreshToken GenerateRefresh(User user);
}

public class TokenService : ITokenService
{
    private readonly JWTOptions _opt;

    public TokenService(IOptions<JWTOptions> opt)
    {
        _opt = opt.Value;
    }

    public string GenerateAccess(User user, out DateTime expires)
    {
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_opt.Key));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        expires = DateTime.UtcNow.AddMinutes(_opt.AccessTokenMinutes);

        var claims = new[]
        {
            new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
            new Claim(JwtRegisteredClaimNames.Email, user.Email)
        };

        var token = new JwtSecurityToken(
            issuer: _opt.Issuer,
            audience: _opt.Audience,
            claims: claims,
            expires: expires,
            signingCredentials: creds
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    public RefreshToken GenerateRefresh(User user)
    {
        var bytes = RandomNumberGenerator.GetBytes(64);

        return new RefreshToken
        {
            Token = Convert.ToBase64String(bytes),
            UserId = user.Id,
            ExpiresAt = DateTime.UtcNow.AddDays(_opt.RefreshTokenDays),
            LastActivityAt = DateTime.UtcNow
        };
    }
}

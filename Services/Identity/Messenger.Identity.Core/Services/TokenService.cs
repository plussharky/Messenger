using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Messenger.Common.Options;
using Messenger.Common.Services;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace Messenger.Identity.Core.Services;

internal sealed class TokenService(
    IOptions<JwtOptions> jwtOptions,
    ITimeProvider timeProvider)
    : ITokenService
{
    private readonly JwtOptions _jwtOptions = jwtOptions.Value;

    public string GenerateAccessToken(Guid userId)
    {
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtOptions.SecretKey));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
        var claims = new List<Claim>
        {
            new (ClaimTypes.NameIdentifier, userId.ToString()),
        };

        var token = new JwtSecurityToken(
            issuer: _jwtOptions.Issuer,
            audience: _jwtOptions.Audience,
            claims: claims,
            expires: timeProvider.GetCurrentTime().UtcDateTime
                .AddMinutes(_jwtOptions.AccessTokenExpirationMinutes),
            signingCredentials: credentials);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}

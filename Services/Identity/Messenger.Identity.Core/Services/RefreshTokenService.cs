using System.Security.Cryptography;
using Messenger.Common.Services;
using Messenger.Identity.Core.Options;
using Messenger.Identity.Core.Repository;
using Messenger.Identity.Core.Repository.Entities;
using Microsoft.Extensions.Options;

namespace Messenger.Identity.Core.Services;

internal sealed class RefreshTokenService(
    IRefreshTokenRepository refreshTokenRepository,
    ITimeProvider timeProvider,
    IOptions<JwtOptions> jwtOptions)
    : IRefreshTokenService
{
    private readonly JwtOptions _jwtOptions = jwtOptions.Value;

    public async Task<RefreshToken> CreateAsync(Guid userId)
    {
        var token = GenerateToken();
        var expiresAt = timeProvider.GetCurrentTime().UtcDateTime.AddDays(_jwtOptions.RefreshTokenExpirationDays);
        var refreshToken = new RefreshToken
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            Token = token,
            ExpiresAt = expiresAt,
            CreatedAt = timeProvider.GetCurrentTime().UtcDateTime,
            IsRevoked = false,
        };
        await refreshTokenRepository.CreateAsync(refreshToken);
        return refreshToken;
    }

    public async Task<RefreshToken?> GetByTokenAsync(string token) =>
        await refreshTokenRepository.GetByTokenAsync(token);

    public async Task RevokeTokenAsync(string token, string? replacedByToken = null) =>
        await refreshTokenRepository.RevokeTokenAsync(token, timeProvider.GetCurrentTime().UtcDateTime, replacedByToken);

    public async Task RevokeAllUserTokensAsync(Guid userId) =>
        await refreshTokenRepository.RevokeAllUserTokensAsync(userId, timeProvider.GetCurrentTime().UtcDateTime);

    public async Task<RefreshToken?> ValidateAndGetTokenAsync(string token)
    {
        var refreshToken = await refreshTokenRepository.GetByTokenAsync(token);
        if (refreshToken == null)
        {
            return null;
        }

        if (refreshToken.IsRevoked)
        {
            return null;
        }

        if (refreshToken.ExpiresAt < timeProvider.GetCurrentTime().UtcDateTime)
        {
            return null;
        }

        return refreshToken;
    }

    private static string GenerateToken()
    {
        var randomNumber = new byte[64];
        using var rng = RandomNumberGenerator.Create();
        rng.GetBytes(randomNumber);
        return Convert.ToBase64String(randomNumber);
    }
}

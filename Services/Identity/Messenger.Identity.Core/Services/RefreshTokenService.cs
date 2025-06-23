using System.Security.Cryptography;
using CSharpFunctionalExtensions;
using Messenger.Common.Services;
using Messenger.Identity.Core.Domain.Errors;
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

    public async Task<Result<RefreshToken, RefreshTokenError>> CreateAsync(Guid userId)
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

        return Result.Success<RefreshToken, RefreshTokenError>(refreshToken);
    }

    public Task<Result<RefreshToken, RefreshTokenError>> GetByTokenAsync(string token)
    {
        return Maybe.From(async () => await refreshTokenRepository.GetByTokenAsync(token))
            .ToResult(RefreshTokenError.TokenNotFound);
    }

    public async Task<UnitResult<RefreshTokenError>> RevokeTokenAsync(string token, string? replacedByToken = null)
    {
        await refreshTokenRepository.RevokeTokenAsync(token, timeProvider.GetCurrentTime().UtcDateTime, replacedByToken);

        return UnitResult.Success<RefreshTokenError>();
    }

    public async Task<UnitResult<RefreshTokenError>> RevokeAllUserTokensAsync(Guid userId)
    {
        await refreshTokenRepository.RevokeAllUserTokensAsync(userId, timeProvider.GetCurrentTime().UtcDateTime);

        return UnitResult.Success<RefreshTokenError>();
    }

    public Task<Result<RefreshToken, RefreshTokenError>> ValidateAndGetTokenAsync(string token)
    {
        return GetByTokenAsync(token)
            .Ensure(t => !t.IsRevoked, RefreshTokenError.TokenRevoked)
            .Ensure(t => t.ExpiresAt >= timeProvider.GetCurrentTime().UtcDateTime, RefreshTokenError.TokenExpired);
    }

    private static string GenerateToken()
    {
        var randomNumber = new byte[64];
        using var rng = RandomNumberGenerator.Create();
        rng.GetBytes(randomNumber);
        return Convert.ToBase64String(randomNumber);
    }
}

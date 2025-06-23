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

    public async Task<Result<RefreshToken, RefreshTokenError>> GetByTokenAsync(string token)
    {
        var refreshToken = await refreshTokenRepository.GetByTokenAsync(token);
        return refreshToken == null
            ? Result.Failure<RefreshToken, RefreshTokenError>(RefreshTokenError.TokenNotFound)
            : Result.Success<RefreshToken, RefreshTokenError>(refreshToken);
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

    public async Task<Result<RefreshToken, RefreshTokenError>> ValidateAndGetTokenAsync(string token)
    {
        var refreshToken = await refreshTokenRepository.GetByTokenAsync(token);
        if (refreshToken == null)
        {
            return Result.Failure<RefreshToken, RefreshTokenError>(RefreshTokenError.TokenNotFound);
        }

        if (refreshToken.IsRevoked)
        {
            return Result.Failure<RefreshToken, RefreshTokenError>(RefreshTokenError.TokenRevoked);
        }

        if (refreshToken.ExpiresAt < timeProvider.GetCurrentTime().UtcDateTime)
        {
            return Result.Failure<RefreshToken, RefreshTokenError>(RefreshTokenError.TokenExpired);
        }

        return Result.Success<RefreshToken, RefreshTokenError>(refreshToken);
    }

    private static string GenerateToken()
    {
        var randomNumber = new byte[64];
        using var rng = RandomNumberGenerator.Create();
        rng.GetBytes(randomNumber);
        return Convert.ToBase64String(randomNumber);
    }
}

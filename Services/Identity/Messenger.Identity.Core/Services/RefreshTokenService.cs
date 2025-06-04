using System.Security.Cryptography;
using Dapper;
using Messenger.Identity.Core.Models;
using Microsoft.Extensions.Options;
using Npgsql;

namespace Messenger.Identity.Core.Services;

internal sealed class RefreshTokenService(
    string connectionString,
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

        using var connection = new NpgsqlConnection(connectionString);
        await connection.OpenAsync();
        await connection.ExecuteAsync(
            "INSERT INTO public.refresh_tokens (id, user_id, token, expires_at, created_at, is_revoked) " +
            "VALUES (@Id, @UserId, @Token, @ExpiresAt, @CreatedAt, @IsRevoked)",
            refreshToken);

        return refreshToken;
    }

    public async Task<RefreshToken?> GetByTokenAsync(string token)
    {
        using var connection = new NpgsqlConnection(connectionString);
        await connection.OpenAsync();
        return await connection.QueryFirstOrDefaultAsync<RefreshToken>(
            "SELECT id, user_id, token, expires_at, created_at, is_revoked, revoked_at, replaced_by_token " +
            "FROM public.refresh_tokens WHERE token = @Token",
            new { Token = token });
    }

    public async Task RevokeTokenAsync(string token, string? replacedByToken = null)
    {
        using var connection = new NpgsqlConnection(connectionString);
        await connection.OpenAsync();
        await connection.ExecuteAsync(
            "UPDATE public.refresh_tokens SET is_revoked = TRUE, revoked_at = @RevokedAt, replaced_by_token = @ReplacedByToken " +
            "WHERE token = @Token",
            new
            {
                Token = token,
                RevokedAt = timeProvider.GetCurrentTime().UtcDateTime,
                ReplacedByToken = replacedByToken,
            });
    }

    public async Task RevokeAllUserTokensAsync(Guid userId)
    {
        using var connection = new NpgsqlConnection(connectionString);
        await connection.OpenAsync();
        await connection.ExecuteAsync(
            "UPDATE public.refresh_tokens SET is_revoked = TRUE, revoked_at = @RevokedAt " +
            "WHERE user_id = @UserId AND is_revoked = FALSE",
            new
            {
                UserId = userId,
                RevokedAt = timeProvider.GetCurrentTime().UtcDateTime,
            });
    }

    public async Task<bool> ValidateTokenAsync(string token)
    {
        var refreshToken = await GetByTokenAsync(token);
        if (refreshToken == null)
        {
            return false;
        }

        if (refreshToken.IsRevoked)
        {
            return false;
        }

        if (refreshToken.ExpiresAt < timeProvider.GetCurrentTime().UtcDateTime)
        {
            return false;
        }

        return true;
    }

    private static string GenerateToken()
    {
        var randomNumber = new byte[64];
        using var rng = RandomNumberGenerator.Create();
        rng.GetBytes(randomNumber);
        return Convert.ToBase64String(randomNumber);
    }
}

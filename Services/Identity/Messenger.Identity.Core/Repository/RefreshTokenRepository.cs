using Dapper;
using Messenger.Identity.Core.Repository.Entities;

namespace Messenger.Identity.Core.Repository;

internal sealed class RefreshTokenRepository(IDbConnectionFactory connectionFactory)
    : IRefreshTokenRepository
{
    public async Task CreateAsync(RefreshToken refreshToken)
    {
        await using var connection = await connectionFactory.OpenConnectionAsync();
        await connection.ExecuteAsync(
            "INSERT INTO public.refresh_tokens (id, user_id, token, expires_at, created_at, is_revoked) " +
            "VALUES (@Id, @UserId, @Token, @ExpiresAt, @CreatedAt, @IsRevoked)",
            new
            {
                Id = refreshToken.Id,
                UserId = refreshToken.UserId,
                Token = refreshToken.Token,
                ExpiresAt = refreshToken.ExpiresAt,
                CreatedAt = refreshToken.CreatedAt,
                IsRevoked = refreshToken.IsRevoked,
            });
    }

    public async Task<RefreshToken?> GetByTokenAsync(string token)
    {
        await using var connection = await connectionFactory.OpenConnectionAsync();
        return await connection.QueryFirstOrDefaultAsync<RefreshToken>(
            "SELECT id, user_id, token, expires_at, created_at, is_revoked, revoked_at, replaced_by_token " +
            "FROM public.refresh_tokens WHERE token = @Token",
            new
            {
                Token = token,
            });
    }

    public async Task RevokeTokenAsync(string token, DateTimeOffset revokedAt, string? replacedByToken = null)
    {
        await using var connection = await connectionFactory.OpenConnectionAsync();
        await connection.ExecuteAsync(
            "UPDATE public.refresh_tokens SET is_revoked = TRUE, revoked_at = @RevokedAt, replaced_by_token = @ReplacedByToken " +
            "WHERE token = @Token",
            new
            {
                Token = token,
                RevokedAt = revokedAt,
                ReplacedByToken = replacedByToken,
            });
    }

    public async Task RevokeAllUserTokensAsync(Guid userId, DateTimeOffset revokedAt)
    {
        await using var connection = await connectionFactory.OpenConnectionAsync();
        await connection.ExecuteAsync(
            "UPDATE public.refresh_tokens SET is_revoked = TRUE, revoked_at = @RevokedAt " +
            "WHERE user_id = @UserId AND is_revoked = FALSE",
            new
            {
                UserId = userId,
                RevokedAt = revokedAt,
            });
    }

    public async Task<int> DeleteExpiredTokensAsync(DateTimeOffset untilTime)
    {
        await using var connection = await connectionFactory.OpenConnectionAsync();
        return await connection.ExecuteAsync(
            "DELETE FROM public.refresh_tokens WHERE expires_at < @UntilTime",
            new
            {
                UntilTime = untilTime,
            });
    }
}

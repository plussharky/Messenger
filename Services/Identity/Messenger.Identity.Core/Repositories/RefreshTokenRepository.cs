using Dapper;
using Messenger.Identity.Core.Entities;
using Messenger.Identity.Core.Options;
using Npgsql;

namespace Messenger.Identity.Core.Repositories;

internal sealed class RefreshTokenRepository(ConnectionString connectionString)
    : IRefreshTokenRepository
{
    public async Task CreateAsync(RefreshToken refreshToken)
    {
        using var connection = new NpgsqlConnection(connectionString.Value);
        await connection.OpenAsync();
        await connection.ExecuteAsync(
            "INSERT INTO public.refresh_tokens (id, user_id, token, expires_at, created_at, is_revoked) " +
            "VALUES (@Id, @UserId, @Token, @ExpiresAt, @CreatedAt, @IsRevoked)",
            refreshToken);
    }

    public async Task<RefreshToken?> GetByTokenAsync(string token)
    {
        using var connection = new NpgsqlConnection(connectionString.Value);
        await connection.OpenAsync();
        return await connection.QueryFirstOrDefaultAsync<RefreshToken>(
            "SELECT id, user_id, token, expires_at, created_at, is_revoked, revoked_at, replaced_by_token " +
            "FROM public.refresh_tokens WHERE token = @Token",
            new
            {
                Token = token,
            });
    }

    public async Task RevokeTokenAsync(string token, DateTime revokedAt, string? replacedByToken = null)
    {
        using var connection = new NpgsqlConnection(connectionString.Value);
        await connection.OpenAsync();
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

    public async Task RevokeAllUserTokensAsync(Guid userId, DateTime revokedAt)
    {
        using var connection = new NpgsqlConnection(connectionString.Value);
        await connection.OpenAsync();
        await connection.ExecuteAsync(
            "UPDATE public.refresh_tokens SET is_revoked = TRUE, revoked_at = @RevokedAt " +
            "WHERE user_id = @UserId AND is_revoked = FALSE",
            new
            {
                UserId = userId,
                RevokedAt = revokedAt,
            });
    }
}

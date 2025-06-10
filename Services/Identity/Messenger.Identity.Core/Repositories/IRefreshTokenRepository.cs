using Messenger.Identity.Core.Entities;

namespace Messenger.Identity.Core.Repositories;

public interface IRefreshTokenRepository
{
    Task CreateAsync(RefreshToken refreshToken);

    Task<RefreshToken?> GetByTokenAsync(string token);

    Task RevokeTokenAsync(string token, DateTimeOffset revokedAt, string? replacedByToken = null);

    Task RevokeAllUserTokensAsync(Guid userId, DateTimeOffset revokedAt);
}

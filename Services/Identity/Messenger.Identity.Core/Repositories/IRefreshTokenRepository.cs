using Messenger.Identity.Core.Entities;

namespace Messenger.Identity.Core.Repositories;

public interface IRefreshTokenRepository
{
    Task CreateAsync(RefreshToken refreshToken);

    Task<RefreshToken?> GetByTokenAsync(string token);

    Task RevokeTokenAsync(string token, DateTime revokedAt, string? replacedByToken = null);

    Task RevokeAllUserTokensAsync(Guid userId, DateTime revokedAt);
}

using Messenger.Identity.Core.Repository.Entities;

namespace Messenger.Identity.Core.Services;

public interface IRefreshTokenService
{
    Task<RefreshToken> CreateAsync(Guid userId);

    Task<RefreshToken?> GetByTokenAsync(string token);

    Task RevokeTokenAsync(string token, string? replacedByToken = null);

    Task RevokeAllUserTokensAsync(Guid userId);

    Task<RefreshToken?> ValidateAndGetTokenAsync(string token);
}

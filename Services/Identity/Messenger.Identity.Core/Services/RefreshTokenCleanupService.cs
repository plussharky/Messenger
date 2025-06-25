using Messenger.Common.Services;
using Messenger.Identity.Core.Repository;

namespace Messenger.Identity.Core.Services;

internal sealed class RefreshTokenCleanupService(IRefreshTokenRepository refreshTokenRepository, ITimeProvider timeProvider)
    : IRefreshTokenCleanupService
{
    public async Task<int> CleanupExpiredTokensAsync()
    {
        var currentTime = timeProvider.GetCurrentTime();
        return await refreshTokenRepository.DeleteExpiredTokensAsync(currentTime);
    }
}

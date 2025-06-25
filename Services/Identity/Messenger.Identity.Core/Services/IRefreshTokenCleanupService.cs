namespace Messenger.Identity.Core.Services;

public interface IRefreshTokenCleanupService
{
    Task<int> CleanupExpiredTokensAsync();
}

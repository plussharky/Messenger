using Hangfire;

namespace Messenger.Identity.Core.Services;

public static class RefreshTokenCleanupJob
{
    public static void ScheduleDailyCleanup()
    {
        RecurringJob.AddOrUpdate<IRefreshTokenCleanupService>(
            "cleanup-expired-refresh-tokens",
            service => service.CleanupExpiredTokensAsync(),
            Cron.Daily);
    }
}

using Hangfire;
using Messenger.Identity.Core.Services;

namespace Messenger.Identity.Api.Services;

internal sealed class RecurringJobsHostedService(IRecurringJobManager recurringJobManager)
    : IHostedService
{
    public Task StartAsync(CancellationToken cancellationToken)
    {
        recurringJobManager.AddOrUpdate<IRefreshTokenCleanupService>(
            "cleanup-expired-refresh-tokens",
            service => service.CleanupExpiredTokensAsync(),
            Cron.Daily);

        return Task.CompletedTask;
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }
}

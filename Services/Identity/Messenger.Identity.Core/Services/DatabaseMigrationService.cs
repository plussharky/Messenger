using FluentMigrator.Runner;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Messenger.Identity.Core.Services;

internal sealed class DatabaseMigrationService(
    IServiceProvider serviceProvider,
    ILogger<DatabaseMigrationService> logger)
    : IHostedService
{
    public Task StartAsync(CancellationToken cancellationToken)
    {
        try
        {
            using var scope = serviceProvider.CreateScope();
            var runner = scope.ServiceProvider.GetRequiredService<IMigrationRunner>();

            logger.LogInformation("Starting database migration...");
            runner.MigrateUp();
            logger.LogInformation("Database migration completed successfully");
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "An error occurred while migrating the database");
            throw;
        }

        return Task.CompletedTask;
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }
}

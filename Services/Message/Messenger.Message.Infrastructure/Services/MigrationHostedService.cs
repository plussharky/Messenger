using Messenger.Messages.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Messenger.Messages.Infrastructure.Services;

internal sealed class MigrationHostedService(
        IServiceProvider serviceProvider,
        ILogger<MigrationHostedService> logger)
    : IHostedService
{
    private readonly IServiceProvider _serviceProvider = serviceProvider;
    private readonly ILogger<MigrationHostedService> _logger = logger;

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        try
        {
            using var scope = _serviceProvider.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<MessageContext>();

            _logger.LogInformation("Starting database migration...");
            await context.Database.MigrateAsync(cancellationToken);
            _logger.LogInformation("Database migration completed successfully");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while migrating the database");
            throw;
        }
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }
}

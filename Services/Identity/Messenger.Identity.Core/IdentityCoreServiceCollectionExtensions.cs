using FluentMigrator.Runner;
using FluentMigrator.Runner.VersionTableInfo;
using Messenger.Identity.Core.Services;
using Microsoft.Extensions.DependencyInjection;

namespace Messenger.Identity.Core;

public static class IdentityCoreServiceCollectionExtensions
{
    public static IServiceCollection AddIdentityCoreServices(this IServiceCollection services, string connectionString)
    {
        services
            .AddFluentMigratorCore()
            .ConfigureRunner(rb => rb
                .AddPostgres()
                .WithGlobalConnectionString(connectionString)
                .WithGlobalCommandTimeout(TimeSpan.FromMinutes(5))
                .ScanIn(typeof(DatabaseMigrationService).Assembly).For.Migrations())
            .AddLogging(lb => lb.AddFluentMigratorConsole())
            .AddScoped<IVersionTableMetaData, CustomVersionTableMetaData>();

        services.AddHostedService<DatabaseMigrationService>();
        services.AddSingleton(connectionString);
        services.AddScoped<IUserService, UserService>();
        services.AddSingleton<ITimeProvider, SystemTimeProvider>();

        return services;
    }
}

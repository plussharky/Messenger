using Dapper;
using FluentMigrator.Runner;
using FluentMigrator.Runner.VersionTableInfo;
using Messenger.Common.Options;
using Messenger.Common.Services;
using Messenger.Identity.Core.Options;
using Messenger.Identity.Core.Repository;
using Messenger.Identity.Core.Repository.Migrations;
using Messenger.Identity.Core.Services;
using Microsoft.Extensions.DependencyInjection;

namespace Messenger.Identity.Core;

public static class IdentityCoreServiceCollectionExtensions
{
    public static IServiceCollection AddIdentityCoreServices(
        this IServiceCollection services, Action<JwtOptions> configureJwt)
    {
        DefaultTypeMap.MatchNamesWithUnderscores = true;

        services
            .AddFluentMigratorCore()
            .ConfigureRunner(
                rb => rb
                    .AddPostgres()
                    .WithGlobalConnectionString(
                        sp => sp.GetRequiredService<ConnectionString>().Value)
                    .WithGlobalCommandTimeout(TimeSpan.FromMinutes(5))
                    .ScanIn(typeof(DatabaseMigrationService).Assembly).For.Migrations())
            .AddLogging(lb => lb.AddFluentMigratorConsole())
            .AddScoped<IVersionTableMetaData, CustomVersionTableMetaData>();

        services.AddHostedService<DatabaseMigrationService>();
        services.AddSingleton<IDbConnectionFactory, DbConnectionFactory>();
        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<IRefreshTokenRepository, RefreshTokenRepository>();
        services.AddSingleton<ITimeProvider, SystemTimeProvider>();
        services.AddScoped<IPasswordHasher, BCryptPasswordHasher>();
        services.AddScoped<IUserService, UserService>();
        services.AddScoped<IRefreshTokenService, RefreshTokenService>();
        services.AddScoped<ITokenService, TokenService>();
        services.AddScoped<IIdentityService, IdentityService>();
        services.AddScoped<IRefreshTokenCleanupService, RefreshTokenCleanupService>();

        services.Configure(configureJwt);
        return services;
    }
}

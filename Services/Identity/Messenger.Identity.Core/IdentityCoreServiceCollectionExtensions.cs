using Dapper;
using FluentMigrator.Runner;
using FluentMigrator.Runner.VersionTableInfo;
using Messenger.Identity.Core.BusinessLogic;
using Messenger.Identity.Core.Repositories;
using Messenger.Identity.Core.Services;
using Microsoft.Extensions.DependencyInjection;

namespace Messenger.Identity.Core;

public static class IdentityCoreServiceCollectionExtensions
{
    public static IServiceCollection AddIdentityCoreServices(
        this IServiceCollection services, string connectionString, Action<JwtOptions> configureJwt)
    {
        DefaultTypeMap.MatchNamesWithUnderscores = true;

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
        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<IRefreshTokenRepository, RefreshTokenRepository>();
        services.AddSingleton<ITimeProvider, SystemTimeProvider>();
        services.AddScoped<IUserService, UserService>();
        services.AddScoped<IRefreshTokenService, RefreshTokenService>();
        services.AddScoped<ITokenService, TokenService>();
        services.AddScoped<IIdentityService, IdentityService>();
        services.Configure(configureJwt);
        return services;
    }
}

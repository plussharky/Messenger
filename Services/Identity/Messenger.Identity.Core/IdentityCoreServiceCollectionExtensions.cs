using System.Globalization;
using Dapper;
using FluentMigrator.Runner;
using FluentMigrator.Runner.VersionTableInfo;
using Messenger.Identity.Core.BusinessLogic;
using Messenger.Identity.Core.Repositories;
using Messenger.Identity.Core.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Messenger.Identity.Core;

public static class IdentityCoreServiceCollectionExtensions
{
    public static IServiceCollection AddIdentityCoreServices(
        this IServiceCollection services, string connectionString, IConfigurationSection jwtSection)
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
        services.Configure<JwtOptions>(options =>
        {
            options.SecretKey = jwtSection["SecretKey"] ?? string.Empty;
            options.Issuer = jwtSection["Issuer"] ?? string.Empty;
            options.Audience = jwtSection["Audience"] ?? string.Empty;
            options.AccessTokenExpirationMinutes = int.TryParse(jwtSection["AccessTokenExpirationMinutes"], CultureInfo.InvariantCulture, out var access) ? access : 60;
            options.RefreshTokenExpirationDays = int.TryParse(jwtSection["RefreshTokenExpirationDays"], CultureInfo.InvariantCulture, out var refresh) ? refresh : 7;
        });
        return services;
    }
}

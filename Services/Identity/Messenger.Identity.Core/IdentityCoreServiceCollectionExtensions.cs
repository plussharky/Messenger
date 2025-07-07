using Dapper;
using FluentMigrator.Runner;
using FluentMigrator.Runner.VersionTableInfo;
using MassTransit;
using Messenger.Common.Options;
using Messenger.Common.Services;
using Messenger.Identity.Core.Options;
using Messenger.Identity.Core.Repository;
using Messenger.Identity.Core.Repository.Migrations;
using Messenger.Identity.Core.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace Messenger.Identity.Core;

public static class IdentityCoreServiceCollectionExtensions
{
    public static IServiceCollection AddIdentityCoreServices(
        this IServiceCollection services,
        Action<JwtOptions> configureJwt,
        Action<RabbitMQOptions> configureRabbitMQ)
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

        services.AddMassTransit(x =>
        {
            x.UsingRabbitMq((context, cfg) =>
            {
                var options = context.GetRequiredService<IOptions<RabbitMQOptions>>().Value;
                var uri = new Uri($"rabbitmq://{options.Username}:{options.Password}@{options.Host}:{options.Port}/{options.VirtualHost}");
                cfg.Host(uri);
                cfg.ConfigureEndpoints(context);
            });
        });

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
        services.AddScoped<IEventPublisher, EventPublisher>();

        services.Configure(configureJwt);
        services.Configure(configureRabbitMQ);

        return services;
    }
}

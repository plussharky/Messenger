using FluentMigrator.Runner;
using FluentMigrator.Runner.VersionTableInfo;
using Messenger.Common.Services;
using Messenger.Identity.Core.Options;
using Messenger.Identity.Core.Repository;
using Messenger.Identity.Core.Repository.Entities;
using Messenger.Identity.Core.Repository.Migrations;
using Messenger.Identity.Core.Services;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Npgsql;
using Respawn;
using Testcontainers.PostgreSql;
using Xunit;

namespace Messenger.Identity.ComponentTests;

public sealed class IdentityWebApplicationFactory
    : WebApplicationFactory<Program>, IAsyncLifetime
{
    private readonly PostgreSqlContainer _postgresContainer;
    private Respawner _respawner = null!;
    private string _connectionString = null!;
    private IServiceScope _scope = null!;
    private IUserRepository _userRepository = null!;
    private IRefreshTokenRepository _refreshTokenRepository = null!;
    private IPasswordHasher _passwordHasher = null!;
    private ITimeProvider _timeProvider = null!;

    public IdentityWebApplicationFactory()
    {
        _postgresContainer = new PostgreSqlBuilder()
            .WithImage("postgres:16-alpine")
            .WithDatabase("messenger")
            .WithUsername("postgres")
            .WithPassword("postgres")
            .Build();
    }

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureServices(services =>
        {
            var dbConnectionFactorydescriptor = services.SingleOrDefault(
                d => d.ServiceType == typeof(IDbConnectionFactory));

            if (dbConnectionFactorydescriptor != null)
            {
                services.Remove(dbConnectionFactorydescriptor);
            }

            var migrationServices = services.Where(d =>
                d.ServiceType.Namespace?.StartsWith("FluentMigrator", StringComparison.Ordinal) == true ||
                d.ImplementationType?.Namespace?.StartsWith("FluentMigrator", StringComparison.Ordinal) == true).ToList();

            foreach (var service in migrationServices)
            {
                services.Remove(service);
            }

            services.AddSingleton<IDbConnectionFactory>(_ => new DbConnectionFactory(new ConnectionString()
            {
                Value = _connectionString,
            }));

            services
                .AddFluentMigratorCore()
                .ConfigureRunner(rb => rb
                    .AddPostgres()
                    .WithGlobalConnectionString(_connectionString)
                    .WithGlobalCommandTimeout(TimeSpan.FromMinutes(5))
                    .ScanIn(typeof(DatabaseMigrationService).Assembly).For.Migrations())
                .AddLogging(lb => lb.AddFluentMigratorConsole())
                .AddScoped<IVersionTableMetaData, CustomVersionTableMetaData>();
        });
    }

    public async Task InitializeAsync()
    {
        await _postgresContainer.StartAsync();
        _connectionString = _postgresContainer.GetConnectionString();

        _scope = Services.CreateScope();
        _userRepository = _scope.ServiceProvider.GetRequiredService<IUserRepository>();
        _refreshTokenRepository = _scope.ServiceProvider.GetRequiredService<IRefreshTokenRepository>();
        _passwordHasher = _scope.ServiceProvider.GetRequiredService<IPasswordHasher>();
        _timeProvider = _scope.ServiceProvider.GetRequiredService<ITimeProvider>();

        var runner = _scope.ServiceProvider.GetRequiredService<IMigrationRunner>();
        runner.MigrateUp();

        using var connection = new NpgsqlConnection(_connectionString);
        await connection.OpenAsync();

        _respawner = await Respawner.CreateAsync(connection, new RespawnerOptions
        {
            DbAdapter = DbAdapter.Postgres,
            SchemasToInclude = new[] { "public" },
        });
    }

    public async Task SeedDatabaseAsync()
    {
        var now = _timeProvider.GetCurrentTime();

        var userId = await _userRepository.CreateUserAsync(
            TestData.User.Email,
            _passwordHasher.HashPassword(TestData.User.Password),
            now);

        var refreshToken = new RefreshToken
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            Token = TestData.RefreshToken.Valid,
            ExpiresAt = now.AddDays(7),
            CreatedAt = now,
            IsRevoked = false,
        };

        var usedRefreshToken = new RefreshToken
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            Token = TestData.RefreshToken.Used,
            ExpiresAt = now.AddDays(7),
            CreatedAt = now,
            IsRevoked = true,
            RevokedAt = now,
            ReplacedByToken = TestData.RefreshToken.New,
        };

        await _refreshTokenRepository.CreateAsync(refreshToken);
        await _refreshTokenRepository.CreateAsync(usedRefreshToken);
    }

    public async Task ResetDatabaseAsync()
    {
        using var connection = new NpgsqlConnection(_connectionString);
        await connection.OpenAsync();
        await _respawner.ResetAsync(connection);
        await SeedDatabaseAsync();
    }

    public new async Task DisposeAsync()
    {
        _scope.Dispose();
        await _postgresContainer.DisposeAsync();
    }
}

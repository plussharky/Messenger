using Messenger.Messages.Domain.Repositories;
using Messenger.Messages.Infrastructure.Data;
using Messenger.Messages.Infrastructure.Services;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Messenger.Messages.ComponentTests;

public sealed class MessengerWebApplicationFactory : WebApplicationFactory<Program>
{
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureServices(services =>
        {
            var descriptor1 = services.SingleOrDefault(d =>
                d.ServiceType == typeof(IDbContextOptionsConfiguration<MessageContext>)) !;

            var descriptor2 = services.SingleOrDefault(d =>
                d.ServiceType == typeof(IHostedService) && d.ImplementationType == typeof(MigrationHostedService)) !;

            services.Remove(descriptor1);
            services.Remove(descriptor2);

            services.AddDbContext<MessageContext>(options =>
            {
                options.UseInMemoryDatabase("InMemoryDbForTesting");
            });

            services.AddScoped<IMessageRepository, MessageRepository>();
        });
    }

    public async Task ResetDatabaseAsync()
    {
        using var scope = Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<MessageContext>();
        await db.Database.EnsureDeletedAsync();
        await db.Database.EnsureCreatedAsync();
    }
}

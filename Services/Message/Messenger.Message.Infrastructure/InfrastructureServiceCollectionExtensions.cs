using Messenger.Messages.Domain.Repositories;
using Messenger.Messages.Infrastructure.Data;
using Messenger.Messages.Infrastructure.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Messenger.Messages.Infrastructure;

public static class InfrastructureServiceCollectionExtensions
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, string connectionString)
    {
        services.AddDbContext<MessageContext>(options =>
            options.UseNpgsql(connectionString));

        services.AddHostedService<MigrationHostedService>();

        services.AddScoped<IMessageRepository, MessageRepository>();

        return services;
    }
}

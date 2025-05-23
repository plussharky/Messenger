using Messenger.Messages.Domain.Repositories;
using Messenger.Messages.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Messenger.Messages.Infrastructure;

public static class InfrastructureServiceCollectionExtensions
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, string? connectionString)
    {
        if (connectionString == null)
        {
            throw new ArgumentNullException(nameof(connectionString));
        }

        services.AddDbContext<MessageContext>(options =>
            options.UseNpgsql(connectionString));

        services.AddScoped<IMessageRepository, MessageRepository>();

        return services;
    }
}

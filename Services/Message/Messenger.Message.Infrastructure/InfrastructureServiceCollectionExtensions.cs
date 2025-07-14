using MassTransit;
using Messenger.Common.Options;
using Messenger.Messages.Consumers;
using Messenger.Messages.Domain.Repositories;
using Messenger.Messages.Infrastructure.Data;
using Messenger.Messages.Infrastructure.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace Messenger.Messages.Infrastructure;

public static class InfrastructureServiceCollectionExtensions
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        string connectionString,
        Action<RabbitMQOptions> configureRabbitMQ)
    {
        services.AddDbContext<Data.MessageContext>(options =>
            options.UseNpgsql(connectionString));

        services.AddHostedService<MigrationHostedService>();

        services.AddScoped<IMessageRepository, MessageRepository>();

        services.Configure(configureRabbitMQ);
        services.AddMassTransit(x =>
        {
            x.AddMessageConsumers();

            x.UsingRabbitMq((context, cfg) =>
            {
                var options = context.GetRequiredService<IOptions<RabbitMQOptions>>().Value;
                cfg.Host(options.Uri);
                cfg.ConfigureEndpoints(context);
            });
        });

        return services;
    }
}

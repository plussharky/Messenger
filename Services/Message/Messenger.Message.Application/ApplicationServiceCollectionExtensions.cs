using Messenger.Message.Application.Services;
using Microsoft.Extensions.DependencyInjection;

namespace Messenger.Message.Application;

public static class ApplicationServiceCollectionExtensions
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddScoped<IMessageService, MessageService>();
        return services;
    }
}

using Messenger.Message.Application.Mappings;
using Messenger.Message.Application.Services;
using Microsoft.Extensions.DependencyInjection;

namespace Messenger.Message.Application;

public static class ApplicationServiceCollectionExtensions
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddScoped<IMessageService, MessageService>();
        services.AddSingleton<ITimeProvider, SystemTimeProvider>();
        services.AddAutoMapper(typeof(MessageProfile));
        return services;
    }
}

using Messenger.Common.Services;
using Messenger.Messages.Application.Services;
using Microsoft.Extensions.DependencyInjection;

namespace Messenger.Messages.Application;

public static class ApplicationServiceCollectionExtensions
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddScoped<IMessageService, MessageService>();
        services.AddSingleton<ITimeProvider, SystemTimeProvider>();
        services.AddScoped<IUserLoginHandler, UserLoginHandler>();
        return services;
    }
}

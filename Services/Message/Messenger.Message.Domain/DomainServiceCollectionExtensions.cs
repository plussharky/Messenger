using Microsoft.Extensions.DependencyInjection;

namespace Messenger.Message.Domain;

public static class DomainServiceCollectionExtensions
{
    public static IServiceCollection AddDomain(this IServiceCollection services)
    {
        return services;
    }
}

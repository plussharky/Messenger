using Microsoft.Extensions.DependencyInjection;

namespace Messenger.Messages.Domain;

public static class DomainServiceCollectionExtensions
{
    public static IServiceCollection AddDomain(this IServiceCollection services)
    {
        return services;
    }
}

using MassTransit;
using Messenger.Messages.Consumers.Consumers;

namespace Messenger.Messages.Consumers;

public static class ConsumerServiceCollectionExtensions
{
    public static IBusRegistrationConfigurator AddMessageConsumers(this IBusRegistrationConfigurator configurator)
    {
        configurator.AddConsumer<UserLoggedInConsumer>();
        return configurator;
    }
}

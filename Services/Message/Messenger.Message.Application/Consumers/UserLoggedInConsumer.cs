using MassTransit;
using Messenger.Common.Events;
using Messenger.Messages.Application.Requests;
using Messenger.Messages.Application.Services;

namespace Messenger.Messages.Application.Consumers;

public sealed class UserLoggedInConsumer(IMessageService messageService)
    : IConsumer<UserLoggedIn>
{
    public async Task Consume(ConsumeContext<UserLoggedIn> context)
    {
        var message = new SendMessageRequest
        {
            Id = Guid.NewGuid(),
            Text = $"Привет, {context.Message.Email}!",
        };

        await messageService.SendMessageAsync(message);
    }
}

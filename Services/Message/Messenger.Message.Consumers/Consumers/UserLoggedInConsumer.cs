using MassTransit;
using Messenger.Identity.Events;
using Messenger.Messages.Application.Requests;
using Messenger.Messages.Application.Services;

namespace Messenger.Messages.Consumers.Consumers;

internal sealed class UserLoggedInConsumer(IUserLoginHandler userLoginHandler)
    : IConsumer<UserLoggedIn>
{
    public async Task Consume(ConsumeContext<UserLoggedIn> context)
    {
        var request = new UserLoginRequest
        {
            UserEmail = context.Message.Email,
            MessageId = context.MessageId ?? Guid.NewGuid(),
        };

        await userLoginHandler.HandleUserLoginAsync(request);
    }
}

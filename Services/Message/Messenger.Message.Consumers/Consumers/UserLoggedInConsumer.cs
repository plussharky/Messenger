using MassTransit;
using Messenger.Identity.Events;
using Messenger.Messages.Application.Requests;
using Messenger.Messages.Application.Services;

namespace Messenger.Messages.Consumers.Consumers;

public sealed class UserLoggedInConsumer(IUserLoginHandler userLoginHandler)
    : IConsumer<UserLoggedIn>
{
    public async Task Consume(ConsumeContext<UserLoggedIn> context)
    {
        var request = new UserLoginRequest
        {
            UserEmail = context.Message.Email,
            UserId = Guid.NewGuid(),
        };

        await userLoginHandler.HandleUserLoginAsync(request);
    }
}

using Messenger.Messages.Application.Requests;

namespace Messenger.Messages.Application.Services;

internal sealed class UserLoginHandler(IMessageService messageService)
    : IUserLoginHandler
{
    public async Task HandleUserLoginAsync(UserLoginRequest request)
    {
        var welcomeMessage = new SendMessageRequest
        {
            Id = request.MessageId,
            Text = $"Привет, {request.UserEmail}! Добро пожаловать в систему.",
        };

        await messageService.SendMessageAsync(welcomeMessage);
    }
}

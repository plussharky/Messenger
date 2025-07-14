using Messenger.Messages.Application.Requests;

namespace Messenger.Messages.Application.Services;

public interface IUserLoginHandler
{
    Task HandleUserLoginAsync(UserLoginRequest request);
}

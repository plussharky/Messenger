namespace Messenger.Messages.Application.Requests;

public sealed class UserLoginRequest
{
    public required string UserEmail { get; init; }

    public required Guid MessageId { get; init; }
}

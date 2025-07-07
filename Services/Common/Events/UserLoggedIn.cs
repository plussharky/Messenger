namespace Messenger.Common.Events;

public record UserLoggedIn
{
    public required string Email { get; init; }
}

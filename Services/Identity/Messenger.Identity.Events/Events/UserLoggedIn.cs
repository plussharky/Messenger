namespace Messenger.Identity.Events;

public record UserLoggedIn
{
    public required string Email { get; init; }
}

namespace Messenger.Identity.Core.Models;

public sealed class User
{
    public Guid Id { get; init; }

    public DateTimeOffset CreatedAt { get; init; }
}

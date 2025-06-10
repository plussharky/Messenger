namespace Messenger.Identity.Core.Repository.Entities;

public sealed class User
{
    public Guid Id { get; init; }

    public DateTimeOffset CreatedAt { get; init; }
}

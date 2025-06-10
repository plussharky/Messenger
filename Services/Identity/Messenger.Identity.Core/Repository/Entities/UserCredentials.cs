namespace Messenger.Identity.Core.Repository.Entities;

public sealed class UserCredentials
{
    public required Guid UserId { get; init; }

    public required string Email { get; init; }

    public required string PasswordHash { get; init; }

    public required string Salt { get; init; }
}

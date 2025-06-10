namespace Messenger.Identity.Core.Repository.Entities;

public sealed class RefreshToken
{
    public required Guid Id { get; init; }

    public required Guid UserId { get; init; }

    public required string Token { get; init; }

    public required DateTimeOffset ExpiresAt { get; init; }

    public required DateTimeOffset CreatedAt { get; init; }

    public required bool IsRevoked { get; init; }

    public DateTimeOffset? RevokedAt { get; init; }

    public string? ReplacedByToken { get; init; }
}

namespace Messenger.Identity.Core.Entities;

public sealed class RefreshToken
{
    public required Guid Id { get; init; }

    public required Guid UserId { get; init; }

    public required string Token { get; init; }

    public required DateTime ExpiresAt { get; init; }

    public required DateTime CreatedAt { get; init; }

    public required bool IsRevoked { get; init; }

    public DateTime? RevokedAt { get; init; }

    public string? ReplacedByToken { get; init; }
}

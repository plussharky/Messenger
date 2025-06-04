namespace Messenger.Identity.Api.DTOs;

public sealed record RefreshTokenRequestDto
{
    public required string RefreshToken { get; init; }
}

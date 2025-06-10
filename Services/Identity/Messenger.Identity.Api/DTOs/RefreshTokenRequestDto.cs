namespace Messenger.Identity.Api.Dtos;

public sealed record RefreshTokenRequestDto
{
    public required string RefreshToken { get; init; }
}

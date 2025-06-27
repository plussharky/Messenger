namespace ChatClient.Models;

public sealed record RefreshTokenRequestDto
{
    public required string RefreshToken { get; init; }
}

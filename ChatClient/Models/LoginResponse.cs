namespace ChatClient.Models;

public sealed record LoginResponse
{
    public required string AccessToken { get; init; }

    public required string RefreshToken { get; init; }
}

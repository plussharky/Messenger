namespace Messenger.Identity.Core.Models;

public sealed class LoginResponse
{
    public required string AccessToken { get; init; }

    public required string RefreshToken { get; init; }
}

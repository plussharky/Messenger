using ChatClient.Models;

namespace ChatClient.Services;

public interface IAuthService
{
    Task<bool> LoginAsync(LoginRequest request);

    Task LogoutAsync();

    Task<bool> IsAuthenticatedAsync();

    Task<string?> GetAccessTokenAsync();

    Task<string?> GetRefreshTokenAsync();

    Task<bool> RefreshTokenAsync();
}

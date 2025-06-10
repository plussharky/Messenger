using Messenger.Identity.Core.Models;

namespace Messenger.Identity.Core.Services;

public interface IIdentityService
{
    Task<Guid> RegisterUserAsync(string email, string password);

    Task<LoginResponse> LoginAsync(string email, string password);

    Task<LoginResponse> RefreshTokenAsync(string refreshToken);
}

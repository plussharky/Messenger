using Messenger.Identity.Core.BusinessLogic.Models;

namespace Messenger.Identity.Core.BusinessLogic;

public interface IIdentityService
{
    Task<Guid> RegisterUserAsync(string email, string password);

    Task<LoginResponse> LoginAsync(string email, string password);

    Task<LoginResponse> RefreshTokenAsync(string refreshToken);
}

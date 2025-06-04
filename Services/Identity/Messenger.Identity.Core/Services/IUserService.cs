using Messenger.Identity.Core.Entities;

namespace Messenger.Identity.Core.Services;

public interface IUserService
{
    Task<Guid> RegisterUserAsync(string email, string password);

    Task<bool> IsEmailExistsAsync(string email);

    Task<User?> AuthenticateUserAsync(string email, string password);

    Task<User?> GetUserByIdAsync(Guid userId);
}

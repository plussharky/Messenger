using Messenger.Identity.Core.Repository.Entities;

namespace Messenger.Identity.Core.Services;

public interface IUserService
{
    Task<Guid> RegisterUserAsync(string email, string password);

    Task<User?> AuthenticateUserAsync(string email, string password);
}

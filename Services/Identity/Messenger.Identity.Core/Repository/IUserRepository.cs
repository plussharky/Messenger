using Messenger.Identity.Core.Repository.Entities;

namespace Messenger.Identity.Core.Repository;

public interface IUserRepository
{
    Task<bool> IsEmailExistsAsync(string email);

    Task<Guid> CreateUserAsync(string email, string passwordHash, string salt, DateTimeOffset createdAt);

    Task<UserCredentials?> GetUserCredentialsByEmailAsync(string email);

    Task<User?> GetUserByIdAsync(Guid userId);
}

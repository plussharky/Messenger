using Messenger.Common.Services;
using Messenger.Identity.Core.Entities;
using Messenger.Identity.Core.Exceptions;
using Messenger.Identity.Core.Repositories;

namespace Messenger.Identity.Core.Services;

internal sealed class UserService(
    IUserRepository userRepository,
    ITimeProvider timeProvider)
    : IUserService
{
    public async Task<Guid> RegisterUserAsync(string email, string password)
    {
        if (await userRepository.IsEmailExistsAsync(email))
        {
            throw new EmailAlreadyExistsException(email);
        }

        var salt = BCrypt.Net.BCrypt.GenerateSalt();
        var passwordHash = BCrypt.Net.BCrypt.HashPassword(password, salt);
        var createdAt = timeProvider.GetCurrentTime();
        return await userRepository.CreateUserAsync(email, passwordHash, salt, createdAt);
    }

    public async Task<User?> AuthenticateUserAsync(string email, string password)
    {
        var credentials = await userRepository.GetUserCredentialsByEmailAsync(email);
        if (credentials == null)
        {
            return null;
        }

        var isValidPassword = BCrypt.Net.BCrypt.Verify(password, credentials.PasswordHash);
        if (!isValidPassword)
        {
            return null;
        }

        return await userRepository.GetUserByIdAsync(credentials.UserId);
    }
}

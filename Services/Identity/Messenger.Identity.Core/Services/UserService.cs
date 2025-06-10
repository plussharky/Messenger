using Messenger.Common.Services;
using Messenger.Identity.Core.Exceptions;
using Messenger.Identity.Core.Repository;
using Messenger.Identity.Core.Repository.Entities;

namespace Messenger.Identity.Core.Services;

internal sealed class UserService(
    IUserRepository userRepository,
    ITimeProvider timeProvider,
    IPasswordHasher passwordHasher)
    : IUserService
{
    public async Task<Guid> RegisterUserAsync(string email, string password)
    {
        if (await userRepository.IsEmailExistsAsync(email))
        {
            throw new EmailAlreadyExistsException(email);
        }

        var passwordHash = passwordHasher.HashPassword(password);
        var createdAt = timeProvider.GetCurrentTime();
        return await userRepository.CreateUserAsync(email, passwordHash, createdAt);
    }

    public async Task<User?> AuthenticateUserAsync(string email, string password)
    {
        var credentials = await userRepository.GetUserCredentialsByEmailAsync(email);
        if (credentials == null)
        {
            return null;
        }

        var isValidPassword = passwordHasher.VerifyPassword(password, credentials.PasswordHash);
        if (!isValidPassword)
        {
            return null;
        }

        return await userRepository.GetUserByIdAsync(credentials.UserId);
    }
}

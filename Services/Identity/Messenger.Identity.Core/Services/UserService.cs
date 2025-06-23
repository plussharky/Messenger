using CSharpFunctionalExtensions;
using Messenger.Common.Services;
using Messenger.Identity.Core.Domain.Errors;
using Messenger.Identity.Core.Repository;
using Messenger.Identity.Core.Repository.Entities;

namespace Messenger.Identity.Core.Services;

internal sealed class UserService(
    IUserRepository userRepository,
    ITimeProvider timeProvider,
    IPasswordHasher passwordHasher)
    : IUserService
{
    public async Task<Result<Guid, RegisterError>> RegisterUserAsync(string email, string password)
    {
        if (await userRepository.IsEmailExistsAsync(email))
        {
            return Result.Failure<Guid, RegisterError>(RegisterError.EmailAlreadyExists);
        }

        var passwordHash = passwordHasher.HashPassword(password);
        var createdAt = timeProvider.GetCurrentTime();
        var userId = await userRepository.CreateUserAsync(email, passwordHash, createdAt);
        return Result.Success<Guid, RegisterError>(userId);
    }

    public async Task<Result<User, LoginError>> AuthenticateUserAsync(string email, string password)
    {
        var credentials = await userRepository.GetUserCredentialsByEmailAsync(email);
        if (credentials == null)
        {
            return Result.Failure<User, LoginError>(LoginError.EmailNotFound);
        }

        var isValidPassword = passwordHasher.VerifyPassword(password, credentials.PasswordHash);
        if (!isValidPassword)
        {
            return Result.Failure<User, LoginError>(LoginError.InvalidPassword);
        }

        var user = await userRepository.GetUserByIdAsync(credentials.UserId);
        return Result.Success<User, LoginError>(user!);
    }
}

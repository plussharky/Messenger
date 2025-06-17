using CSharpFunctionalExtensions;
using Messenger.Common.Services;
using Messenger.Identity.Core.Repository;
using Messenger.Identity.Core.Repository.Entities;

namespace Messenger.Identity.Core.Services;

internal sealed class UserService(
    IUserRepository userRepository,
    ITimeProvider timeProvider,
    IPasswordHasher passwordHasher)
    : IUserService
{
    public async Task<Result<Guid>> RegisterUserAsync(string email, string password)
    {
        if (await userRepository.IsEmailExistsAsync(email))
        {
            return Result.Failure<Guid>($"Email {email} уже существует");
        }

        var passwordHash = passwordHasher.HashPassword(password);
        var createdAt = timeProvider.GetCurrentTime();
        var userId = await userRepository.CreateUserAsync(email, passwordHash, createdAt);
        return Result.Success(userId);
    }

    public async Task<Result<User>> AuthenticateUserAsync(string email, string password)
    {
        var credentials = await userRepository.GetUserCredentialsByEmailAsync(email);
        if (credentials == null)
        {
            return Result.Failure<User>($"Пользователь с email {email} не найден");
        }

        var isValidPassword = passwordHasher.VerifyPassword(password, credentials.PasswordHash);
        if (!isValidPassword)
        {
            return Result.Failure<User>("Неверный пароль");
        }

        var user = await userRepository.GetUserByIdAsync(credentials.UserId);
        return Result.Success(user!);
    }
}

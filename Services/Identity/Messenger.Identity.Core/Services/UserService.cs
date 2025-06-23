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
        var maybeUser = await Maybe.From(async () => await userRepository.GetUserCredentialsByEmailAsync(email));

        return await maybeUser.Match(
            _ => Task.FromResult(Result.Failure<Guid, RegisterError>(RegisterError.EmailAlreadyExists)),
            async () =>
            {
                var passwordHash = passwordHasher.HashPassword(password);
                var createdAt = timeProvider.GetCurrentTime();
                var userId = await userRepository.CreateUserAsync(email, passwordHash, createdAt);
                return Result.Success<Guid, RegisterError>(userId);
            });
    }

    public async Task<Result<User, LoginError>> AuthenticateUserAsync(string email, string password)
    {
        return await Maybe.From(async () => await userRepository.GetUserCredentialsByEmailAsync(email))
            .ToResult(LoginError.EmailNotFound)
            .Ensure(credentials => passwordHasher.VerifyPassword(password, credentials.PasswordHash), LoginError.InvalidPassword)
            .Map(async credentials => await userRepository.GetUserByIdAsync(credentials.UserId));
    }
}

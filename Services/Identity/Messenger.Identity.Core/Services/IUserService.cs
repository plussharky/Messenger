using CSharpFunctionalExtensions;
using Messenger.Identity.Core.Repository.Entities;

namespace Messenger.Identity.Core.Services;

public interface IUserService
{
    Task<Result<Guid>> RegisterUserAsync(string email, string password);

    Task<Result<User>> AuthenticateUserAsync(string email, string password);
}

using CSharpFunctionalExtensions;
using Messenger.Identity.Core.Domain.Errors;
using Messenger.Identity.Core.Repository.Entities;

namespace Messenger.Identity.Core.Services;

public interface IUserService
{
    Task<Result<Guid, RegisterError>> RegisterUserAsync(string email, string password);

    Task<Result<User, LoginError>> AuthenticateUserAsync(string email, string password);
}

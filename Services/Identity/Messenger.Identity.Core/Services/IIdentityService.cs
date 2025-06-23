using CSharpFunctionalExtensions;
using Messenger.Identity.Core.Domain.Errors;
using Messenger.Identity.Core.Models;

namespace Messenger.Identity.Core.Services;

public interface IIdentityService
{
    Task<Result<Guid, RegisterError>> RegisterUserAsync(string email, string password);

    Task<Result<LoginResponse, LoginError>> LoginAsync(string email, string password);

    Task<Result<LoginResponse, RefreshTokenError>> RefreshTokenAsync(string refreshToken);
}

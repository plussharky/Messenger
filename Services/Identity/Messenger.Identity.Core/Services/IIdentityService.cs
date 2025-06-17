using CSharpFunctionalExtensions;
using Messenger.Identity.Core.Models;

namespace Messenger.Identity.Core.Services;

public interface IIdentityService
{
    Task<Result<Guid>> RegisterUserAsync(string email, string password);

    Task<Result<LoginResponse>> LoginAsync(string email, string password);

    Task<Result<LoginResponse>> RefreshTokenAsync(string refreshToken);
}

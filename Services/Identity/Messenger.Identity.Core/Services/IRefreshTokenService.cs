using CSharpFunctionalExtensions;
using Messenger.Identity.Core.Repository.Entities;

namespace Messenger.Identity.Core.Services;

public interface IRefreshTokenService
{
    Task<Result<RefreshToken>> CreateAsync(Guid userId);

    Task<Result<RefreshToken>> GetByTokenAsync(string token);

    Task<Result> RevokeTokenAsync(string token, string? replacedByToken = null);

    Task<Result> RevokeAllUserTokensAsync(Guid userId);

    Task<Result<RefreshToken>> ValidateAndGetTokenAsync(string token);
}

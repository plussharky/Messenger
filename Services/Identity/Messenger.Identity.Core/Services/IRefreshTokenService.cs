using CSharpFunctionalExtensions;
using Messenger.Identity.Core.Domain.Errors;
using Messenger.Identity.Core.Repository.Entities;

namespace Messenger.Identity.Core.Services;

public interface IRefreshTokenService
{
    Task<Result<RefreshToken, RefreshTokenError>> CreateAsync(Guid userId);

    Task<UnitResult<RefreshTokenError>> RevokeTokenAsync(string token, string? replacedByToken = null);

    Task<UnitResult<RefreshTokenError>> RevokeAllUserTokensAsync(Guid userId);

    Task<Result<RefreshToken, RefreshTokenError>> GetTokenAsync(string token);
}

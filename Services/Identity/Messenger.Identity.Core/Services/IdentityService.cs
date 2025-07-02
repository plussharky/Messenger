using CSharpFunctionalExtensions;
using Messenger.Identity.Core.Domain.Errors;
using Messenger.Identity.Core.Models;

namespace Messenger.Identity.Core.Services;

internal sealed class IdentityService(
    IUserService userService,
    ITokenService tokenService,
    IRefreshTokenService refreshTokenService)
    : IIdentityService
{
    public Task<Result<Guid, RegisterError>> RegisterUserAsync(string email, string password)
    {
        return userService.RegisterUserAsync(email, password);
    }

    public Task<Result<LoginResponse, LoginError>> LoginAsync(string email, string password)
    {
        return userService.AuthenticateUserAsync(email, password)
            .Bind(user => GenerateTokensAsync(user.Id).MapError(_ => LoginError.TokenGenerationFailed));
    }

    public Task<Result<LoginResponse, RefreshTokenError>> RefreshTokenAsync(string refreshToken)
    {
        return refreshTokenService
            .GetTokenAsync(refreshToken)
            .Bind(token => GenerateTokensAsync(token.UserId)
                .Check(pair => refreshTokenService.RevokeTokenAsync(refreshToken, pair.RefreshToken)));
    }

    private Task<Result<LoginResponse, RefreshTokenError>> GenerateTokensAsync(Guid userId)
    {
        return refreshTokenService.CreateAsync(userId)
            .Map(refreshToken => new LoginResponse
            {
                AccessToken = tokenService.GenerateAccessToken(userId),
                RefreshToken = refreshToken.Token,
            });
    }
}

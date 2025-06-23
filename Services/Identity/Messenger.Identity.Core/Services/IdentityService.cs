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
    public async Task<Result<Guid, RegisterError>> RegisterUserAsync(string email, string password)
    {
        return await userService.RegisterUserAsync(email, password);
    }

    public async Task<Result<LoginResponse, LoginError>> LoginAsync(string email, string password)
    {
        var userResult = await userService.AuthenticateUserAsync(email, password);
        if (userResult.IsFailure)
        {
            return Result.Failure<LoginResponse, LoginError>(userResult.Error);
        }

        return await GenerateTokensAsync(userResult.Value.Id)
            .MapError(_ => LoginError.TokenGenerationFailed);
    }

    public async Task<Result<LoginResponse, RefreshTokenError>> RefreshTokenAsync(string refreshToken)
    {
        var tokenResult = await refreshTokenService.ValidateAndGetTokenAsync(refreshToken);
        if (tokenResult.IsFailure)
        {
            return Result.Failure<LoginResponse, RefreshTokenError>(tokenResult.Error);
        }

        var responseResult = await GenerateTokensAsync(tokenResult.Value.UserId);
        await refreshTokenService.RevokeTokenAsync(refreshToken, responseResult.Value.RefreshToken);

        return responseResult;
    }

    private async Task<Result<LoginResponse, RefreshTokenError>> GenerateTokensAsync(Guid userId)
    {
        var accessToken = tokenService.GenerateAccessToken(userId);
        var refreshTokenResult = await refreshTokenService.CreateAsync(userId);
        if (refreshTokenResult.IsFailure)
        {
            return Result.Failure<LoginResponse, RefreshTokenError>(refreshTokenResult.Error);
        }

        return Result.Success<LoginResponse, RefreshTokenError>(new LoginResponse
        {
            AccessToken = accessToken,
            RefreshToken = refreshTokenResult.Value.Token,
        });
    }
}

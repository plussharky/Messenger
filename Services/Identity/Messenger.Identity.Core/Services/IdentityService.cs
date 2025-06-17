using CSharpFunctionalExtensions;
using Messenger.Identity.Core.Models;

namespace Messenger.Identity.Core.Services;

internal sealed class IdentityService(
    IUserService userService,
    ITokenService tokenService,
    IRefreshTokenService refreshTokenService)
    : IIdentityService
{
    public async Task<Result<Guid>> RegisterUserAsync(string email, string password)
    {
        return await userService.RegisterUserAsync(email, password);
    }

    public async Task<Result<LoginResponse>> LoginAsync(string email, string password)
    {
        var userResult = await userService.AuthenticateUserAsync(email, password);
        if (userResult.IsFailure)
        {
            return Result.Failure<LoginResponse>(userResult.Error);
        }

        return await GenerateTokensAsync(userResult.Value.Id);
    }

    public async Task<Result<LoginResponse>> RefreshTokenAsync(string refreshToken)
    {
        var tokenResult = await refreshTokenService.ValidateAndGetTokenAsync(refreshToken);
        if (tokenResult.IsFailure)
        {
            return Result.Failure<LoginResponse>(tokenResult.Error);
        }

        var responseResult = await GenerateTokensAsync(tokenResult.Value.UserId);
        if (responseResult.IsFailure)
        {
            return responseResult;
        }

        var revokeResult = await refreshTokenService.RevokeTokenAsync(refreshToken, responseResult.Value.RefreshToken);
        if (revokeResult.IsFailure)
        {
            return Result.Failure<LoginResponse>(revokeResult.Error);
        }

        return responseResult;
    }

    private async Task<Result<LoginResponse>> GenerateTokensAsync(Guid userId)
    {
        var accessToken = tokenService.GenerateAccessToken(userId);
        var refreshTokenResult = await refreshTokenService.CreateAsync(userId);
        if (refreshTokenResult.IsFailure)
        {
            return Result.Failure<LoginResponse>(refreshTokenResult.Error);
        }

        return Result.Success(new LoginResponse
        {
            AccessToken = accessToken,
            RefreshToken = refreshTokenResult.Value.Token,
        });
    }
}

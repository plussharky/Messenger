using Messenger.Identity.Core.Exceptions;
using Messenger.Identity.Core.Models;

namespace Messenger.Identity.Core.Services;

internal sealed class IdentityService(
    IUserService userService,
    ITokenService tokenService,
    IRefreshTokenService refreshTokenService)
    : IIdentityService
{
    public async Task<Guid> RegisterUserAsync(string email, string password)
    {
        try
        {
            return await userService.RegisterUserAsync(email, password);
        }
        catch (EmailAlreadyExistsException)
        {
            throw;
        }
        catch (Exception ex)
        {
            throw new RegistrationFailedException("Ошибка регистрации пользователя", ex);
        }
    }

    public async Task<LoginResponse> LoginAsync(string email, string password)
    {
        var user = await userService.AuthenticateUserAsync(email, password);
        if (user == null)
        {
            throw new InvalidCredentialsException();
        }

        return await GenerateTokensAsync(user.Id);
    }

    public async Task<LoginResponse> RefreshTokenAsync(string refreshToken)
    {
        var oldToken = await refreshTokenService.ValidateAndGetTokenAsync(refreshToken);
        if (oldToken == null)
        {
            throw new InvalidRefreshTokenException();
        }

        var response = await GenerateTokensAsync(oldToken.UserId);
        await refreshTokenService.RevokeTokenAsync(refreshToken, response.RefreshToken);
        return response;
    }

    private async Task<LoginResponse> GenerateTokensAsync(Guid userId)
    {
        var accessToken = tokenService.GenerateAccessToken(userId);
        var refreshToken = await refreshTokenService.CreateAsync(userId);
        return new LoginResponse
        {
            AccessToken = accessToken,
            RefreshToken = refreshToken.Token,
        };
    }
}

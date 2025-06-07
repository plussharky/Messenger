using System.Security.Claims;
using Messenger.Identity.Core.BusinessLogic.Models;
using Messenger.Identity.Core.Exceptions;
using Messenger.Identity.Core.Services;

namespace Messenger.Identity.Core.BusinessLogic;

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

        var claims = new List<Claim> { new (ClaimTypes.NameIdentifier, user.Id.ToString()) };
        var accessToken = tokenService.GenerateAccessToken(claims);
        var refreshToken = (await refreshTokenService.CreateAsync(user.Id)).Token;
        return new LoginResponse
        {
            AccessToken = accessToken,
            RefreshToken = refreshToken,
        };
    }

    public async Task<LoginResponse> RefreshTokenAsync(string refreshToken)
    {
        if (!await refreshTokenService.ValidateTokenAsync(refreshToken))
        {
            throw new InvalidRefreshTokenException();
        }

        var oldToken = await refreshTokenService.GetByTokenAsync(refreshToken);
        if (oldToken == null)
        {
            throw new InvalidRefreshTokenException();
        }

        var userId = oldToken.UserId;
        var claims = new List<Claim> { new (ClaimTypes.NameIdentifier, userId.ToString()) };
        var accessToken = tokenService.GenerateAccessToken(claims);
        var newRefreshToken = (await refreshTokenService.CreateAsync(userId)).Token;
        await refreshTokenService.RevokeTokenAsync(refreshToken, newRefreshToken);
        return new LoginResponse
        {
            AccessToken = accessToken,
            RefreshToken = newRefreshToken,
        };
    }
}

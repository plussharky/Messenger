using System.Security.Claims;
using Messenger.Identity.Api.DTOs;
using Messenger.Identity.Core.Exceptions;
using Messenger.Identity.Core.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Messenger.Identity.Api.Controllers;

[ApiController]
[Route("api/auth")]
public sealed class AuthController(
    IUserService userService,
    ITokenService tokenService,
    IRefreshTokenService refreshTokenService)
    : ControllerBase
{
    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterRequestDto request)
    {
        try
        {
            var userId = await userService.RegisterUserAsync(request.Email, request.Password);
            return Ok(new { UserId = userId });
        }
        catch (EmailAlreadyExistsException ex)
        {
            return Conflict(new { Error = ex.Message });
        }
        catch (IdentityException ex)
        {
            return BadRequest(new { Error = ex.Message });
        }
        catch
        {
            return StatusCode(500, new { Error = "Внутренняя ошибка сервера" });
        }
    }

    [HttpPost("login")]
    [AllowAnonymous]
    public async Task<IActionResult> Login([FromBody] LoginRequestDto request)
    {
        try
        {
            var user = await userService.AuthenticateUserAsync(request.Email, request.Password);
            if (user == null)
            {
                return Unauthorized(new { Error = "Неверный email или пароль" });
            }

            var claims = new List<Claim>
            {
                new (ClaimTypes.NameIdentifier, user.Id.ToString()),
            };

            var accessToken = tokenService.GenerateAccessToken(claims);
            var refreshToken = (await refreshTokenService.CreateAsync(user.Id)).Token;

            return Ok(new LoginResponseDto
            {
                AccessToken = accessToken,
                RefreshToken = refreshToken,
            });
        }
        catch (IdentityException ex)
        {
            return BadRequest(new { Error = ex.Message });
        }
        catch
        {
            return StatusCode(500, new { Error = "Внутренняя ошибка сервера" });
        }
    }

    [HttpPost("refresh")]
    [AllowAnonymous]
    public async Task<IActionResult> RefreshToken([FromBody] RefreshTokenRequestDto request)
    {
        try
        {
            if (!await refreshTokenService.ValidateTokenAsync(request.RefreshToken))
            {
                return Unauthorized(new { Error = "Недействительный refresh token" });
            }

            var oldToken = await refreshTokenService.GetByTokenAsync(request.RefreshToken);
            if (oldToken == null)
            {
                return Unauthorized(new { Error = "Недействительный refresh token" });
            }

            var userId = oldToken.UserId;

            var claims = new List<Claim>
            {
                new (ClaimTypes.NameIdentifier, userId.ToString()),
            };

            var accessToken = tokenService.GenerateAccessToken(claims);
            var newRefreshToken = (await refreshTokenService.CreateAsync(userId)).Token;

            await refreshTokenService.RevokeTokenAsync(request.RefreshToken, newRefreshToken);

            return Ok(new LoginResponseDto
            {
                AccessToken = accessToken,
                RefreshToken = newRefreshToken,
            });
        }
        catch (IdentityException ex)
        {
            return BadRequest(new { Error = ex.Message });
        }
        catch
        {
            return StatusCode(500, new { Error = "Внутренняя ошибка сервера" });
        }
    }
}

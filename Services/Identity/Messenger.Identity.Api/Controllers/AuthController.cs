using System.Security.Claims;
using Messenger.Identity.Api.DTOs;
using Messenger.Identity.Core.Exceptions;
using Messenger.Identity.Core.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Messenger.Identity.Api.Controllers;

[ApiController]
[Route("api/auth")]
public sealed class AuthController(IUserService userService, ITokenService tokenService)
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
                new (ClaimTypes.Email, request.Email),
            };

            var accessToken = tokenService.GenerateAccessToken(claims);
            var refreshToken = tokenService.GenerateRefreshToken();

            return Ok(new LoginResponseDto()
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
}

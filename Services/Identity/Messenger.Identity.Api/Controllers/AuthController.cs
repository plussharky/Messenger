using Messenger.Identity.Api.DTOs;
using Messenger.Identity.Core.Exceptions;
using Messenger.Identity.Core.Services;
using Microsoft.AspNetCore.Mvc;

namespace Messenger.Identity.Api.Controllers;

[ApiController]
[Route("api/auth")]
public sealed class AuthController(IUserService userService)
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
}

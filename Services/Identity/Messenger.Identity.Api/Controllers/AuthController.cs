using AutoMapper;
using Messenger.Identity.Api.Dtos;
using Messenger.Identity.Core.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Messenger.Identity.Api.Controllers;

[ApiController]
[Route("api/auth")]
public sealed class AuthController(
    IIdentityService identityService,
    IMapper mapper)
    : ControllerBase
{
    [HttpPost("register")]
    [AllowAnonymous]
    public async Task<IActionResult> Register([FromBody] RegisterRequestDto request)
    {
        var result = await identityService.RegisterUserAsync(request.Email, request.Password);
        if (result.IsFailure)
        {
            return Problem(
                statusCode: StatusCodes.Status400BadRequest,
                title: "Ошибка регистрации",
                detail: result.Error,
                instance: HttpContext.Request.Path);
        }

        return Ok(new { UserId = result.Value });
    }

    [HttpPost("login")]
    [AllowAnonymous]
    public async Task<IActionResult> Login([FromBody] LoginRequestDto request)
    {
        var result = await identityService.LoginAsync(request.Email, request.Password);
        if (result.IsFailure)
        {
            return Problem(
                statusCode: StatusCodes.Status400BadRequest,
                title: "Ошибка входа",
                detail: result.Error,
                instance: HttpContext.Request.Path);
        }

        var dto = mapper.Map<LoginResponseDto>(result.Value);
        return Ok(dto);
    }

    [HttpPost("refresh")]
    [AllowAnonymous]
    public async Task<IActionResult> RefreshToken([FromBody] RefreshTokenRequestDto request)
    {
        var result = await identityService.RefreshTokenAsync(request.RefreshToken);
        if (result.IsFailure)
        {
            return Problem(
                statusCode: StatusCodes.Status400BadRequest,
                title: "Ошибка обновления токена",
                detail: result.Error,
                instance: HttpContext.Request.Path);
        }

        var dto = mapper.Map<LoginResponseDto>(result.Value);
        return Ok(dto);
    }
}

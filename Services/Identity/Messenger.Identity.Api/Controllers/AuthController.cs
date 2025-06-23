using AutoMapper;
using Messenger.Identity.Api.Dtos;
using Messenger.Identity.Api.Errors;
using Messenger.Identity.Core.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Messenger.Identity.Api.Controllers;

[ApiController]
[Route("api/auth")]
public sealed class AuthController(
    IIdentityService identityService,
    IErrorHandler errorHandler,
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
            return errorHandler.Handle(result.Error);
        }

        return Ok(new RegisterResponseDto
        {
            UserId = result.Value,
        });
    }

    [HttpPost("login")]
    [AllowAnonymous]
    public async Task<IActionResult> Login([FromBody] LoginRequestDto request)
    {
        var result = await identityService.LoginAsync(request.Email, request.Password);
        if (result.IsFailure)
        {
            return errorHandler.Handle(result.Error);
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
            return errorHandler.Handle(result.Error);
        }

        var dto = mapper.Map<LoginResponseDto>(result.Value);
        return Ok(dto);
    }
}

using AutoMapper;
using Messenger.Identity.Api.DTOs;
using Messenger.Identity.Core.BusinessLogic;
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
        var userId = await identityService.RegisterUserAsync(request.Email, request.Password);
        return Ok(new
        {
            UserId = userId,
        });
    }

    [HttpPost("login")]
    [AllowAnonymous]
    public async Task<IActionResult> Login([FromBody] LoginRequestDto request)
    {
        var result = await identityService.LoginAsync(request.Email, request.Password);
        var dto = mapper.Map<LoginResponseDto>(result);
        return Ok(dto);
    }

    [HttpPost("refresh")]
    [AllowAnonymous]
    public async Task<IActionResult> RefreshToken([FromBody] RefreshTokenRequestDto request)
    {
        var result = await identityService.RefreshTokenAsync(request.RefreshToken);
        var dto = mapper.Map<LoginResponseDto>(result);
        return Ok(dto);
    }
}

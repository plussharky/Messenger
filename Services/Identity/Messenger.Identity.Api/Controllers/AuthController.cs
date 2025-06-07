using AutoMapper;
using Messenger.Identity.Api.DTOs;
using Messenger.Identity.Core.BusinessLogic;
using Messenger.Identity.Core.Exceptions;
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
    public async Task<IActionResult> Register([FromBody] RegisterRequestDto request)
    {
        try
        {
            var userId = await identityService.RegisterUserAsync(request.Email, request.Password);
            return Ok(new { UserId = userId });
        }
        catch (Exception ex)
        {
            return BadRequest(new { Error = ex.Message });
        }
    }

    [HttpPost("login")]
    [AllowAnonymous]
    public async Task<IActionResult> Login([FromBody] LoginRequestDto request)
    {
        try
        {
            var result = await identityService.LoginAsync(request.Email, request.Password);
            var dto = mapper.Map<LoginResponseDto>(result);
            return Ok(dto);
        }
        catch (InvalidCredentialsException ex)
        {
            return Unauthorized(new { Error = ex.Message });
        }
        catch (Exception ex)
        {
            return BadRequest(new { Error = ex.Message });
        }
    }

    [HttpPost("refresh")]
    [AllowAnonymous]
    public async Task<IActionResult> RefreshToken([FromBody] RefreshTokenRequestDto request)
    {
        try
        {
            var result = await identityService.RefreshTokenAsync(request.RefreshToken);
            var dto = mapper.Map<LoginResponseDto>(result);
            return Ok(dto);
        }
        catch (InvalidRefreshTokenException ex)
        {
            return Unauthorized(new { Error = ex.Message });
        }
        catch (Exception ex)
        {
            return BadRequest(new { Error = ex.Message });
        }
    }
}

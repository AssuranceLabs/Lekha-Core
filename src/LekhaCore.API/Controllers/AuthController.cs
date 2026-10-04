using LekhaCore.Application.DTOs.Auth;
using LekhaCore.Application.Interfaces.IService;
using Microsoft.AspNetCore.Mvc;

namespace LekhaCore.Api.Controllers;

[ApiController]
[Route("api/auth")]
public class AuthController(IAuthService authService) : ApiControllerBase
{
    private readonly IAuthService _authService = authService;

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequest request)
    {
        var result = await _authService.LoginAsync(request);
        return ProcessResult(result);
    }

    [HttpPost("refresh")]
    public async Task<IActionResult> Refresh([FromBody] RefreshTokenRequest request)
    {
        var result = await _authService.RefreshTokenAsync(request);
        return ProcessResult(result);
    }

    [HttpPost("logout")]
    public async Task<IActionResult> Logout(string refreshToken)
    {
        var result = await _authService.LogoutAsync(refreshToken);
        return ProcessResult(result);
    }
}

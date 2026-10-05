using LekhaCore.Application.DTOs;
using LekhaCore.Application.DTOs.Auth;
using LekhaCore.Domain.Common;

namespace LekhaCore.Application.Interfaces.IService;

public interface IAuthService
{
    Task<Result<TokenResponseDto>> LoginAsync(LoginRequest request);
    Task<Result<TokenResponseDto>> RefreshTokenAsync(RefreshTokenRequest request);
    Task<Result> LogoutAsync(string refreshToken);
}

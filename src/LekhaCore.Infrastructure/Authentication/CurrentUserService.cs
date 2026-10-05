using LekhaCore.Application.Interfaces.IService;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;

namespace LekhaCore.Infrastructure.Authentication;

public class CurrentUserService(IHttpContextAccessor httpContextAccessor) : ICurrentUserService
{
    public Guid? UserId
    {
        get
        {
            var userId = FindFirst("uid") ?? FindFirst(ClaimTypes.NameIdentifier);
            return Guid.TryParse(userId, out var id) ? id : null;
        }
    }

    public string? Email => FindFirst(ClaimTypes.Email) ?? FindFirst("email");

    public string? Role => FindFirst(ClaimTypes.Role) ?? FindFirst("role");

    public bool IsAuthenticated =>
        httpContextAccessor.HttpContext?.User?.Identity?.IsAuthenticated ?? false;

    private string? FindFirst(string claimType) =>
        httpContextAccessor.HttpContext?.User?.FindFirstValue(claimType);
}

using LekhaCore.Application.Interfaces;
using LekhaCore.Application.Interfaces.IService;
using LekhaCore.Domain.Common.Constants;
using Microsoft.AspNetCore.Http;

namespace LekhaCore.Infrastructure.Authentication;

public sealed class WorkContext(ICurrentUserService currentUser, IHttpContextAccessor httpContextAccessor) : IWorkContext
{
    private string? _actorOverride;

    public string CurrentUserId =>
        _actorOverride
        ?? (currentUser.UserId is Guid id ? id.ToString("D") : AuditActors.System);

    public void SetCurrentUser(Guid userId) => _actorOverride = userId.ToString("D");

    public string? IpAddress =>
        httpContextAccessor.HttpContext?.Connection.RemoteIpAddress?.ToString();

    public string? CorrelationId =>
        httpContextAccessor.HttpContext?.Items["CorrelationId"]?.ToString();
}

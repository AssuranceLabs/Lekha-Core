using System.Security.Claims;

namespace LekhaCore.Application.Interfaces.IService;

public interface IPermissionService
{
    Task<bool> HasPermissionAsync(
        ClaimsPrincipal user,
        string permission,
        CancellationToken cancellationToken = default);

    Task<IReadOnlySet<string>> GetPermissionsAsync(
        ClaimsPrincipal user,
        CancellationToken cancellationToken = default);

    Task<IReadOnlySet<string>> GetPermissionsAsync(
        Guid userId,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyList<string>> GetRoleNamesAsync(
        Guid userId,
        CancellationToken cancellationToken = default);

    void Invalidate();
}

using System.Diagnostics;
using System.Security.Claims;
using LekhaCore.Application.Authorization;
using LekhaCore.Application.Interfaces.IService;
using Microsoft.Extensions.Logging;

namespace LekhaCore.Infrastructure.Authorization;

public sealed class PermissionService(
    IEffectivePermissionSource source,
    ILogger<PermissionService> logger) : IPermissionService
{
    private Guid? _cachedUserId;
    private IReadOnlySet<string>? _cachedPermissions;
    private IReadOnlyList<string>? _cachedRoles;

    public async Task<bool> HasPermissionAsync(
        ClaimsPrincipal user,
        string permission,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(permission))
            return false;

        var permissions = await GetPermissionsAsync(user, cancellationToken);
        return permissions.Contains(permission);
    }

    public async Task<IReadOnlySet<string>> GetPermissionsAsync(
        ClaimsPrincipal user,
        CancellationToken cancellationToken = default)
    {
        var snapshot = await LoadAsync(ReadUserId(user), cancellationToken);
        return snapshot.Permissions;
    }

    public async Task<IReadOnlySet<string>> GetPermissionsAsync(
        Guid userId,
        CancellationToken cancellationToken = default)
    {
        var snapshot = await LoadAsync(userId, cancellationToken);
        return snapshot.Permissions;
    }

    public async Task<IReadOnlyList<string>> GetRoleNamesAsync(
        Guid userId,
        CancellationToken cancellationToken = default)
    {
        var snapshot = await LoadAsync(userId, cancellationToken);
        return snapshot.Roles;
    }

    public void Invalidate()
    {
        _cachedUserId = null;
        _cachedPermissions = null;
        _cachedRoles = null;
    }

    private async Task<PermissionSnapshot> LoadAsync(Guid? userId, CancellationToken cancellationToken)
    {
        if (userId is null)
            return PermissionSnapshot.Empty;

        if (_cachedUserId == userId && _cachedPermissions is not null && _cachedRoles is not null)
        {
            logger.LogDebug(
                "Permission cache hit for user {UserId}. PermissionCount {PermissionCount}.",
                userId,
                _cachedPermissions.Count);
            return new PermissionSnapshot(_cachedPermissions, _cachedRoles);
        }

        var watch = Stopwatch.StartNew();
        var codes = await source.GetPermissionCodesAsync(userId.Value, cancellationToken);
        var roles = await source.GetRoleNamesAsync(userId.Value, cancellationToken);
        watch.Stop();

        var permissions = EffectivePermissions.Union(codes);
        _cachedUserId = userId;
        _cachedPermissions = permissions;
        _cachedRoles = roles;

        logger.LogDebug(
            "Permission cache miss for user {UserId}. Lookup took {ElapsedMilliseconds} ms. PermissionCount {PermissionCount}. RoleCount {RoleCount}.",
            userId,
            watch.ElapsedMilliseconds,
            permissions.Count,
            roles.Count);

        return new PermissionSnapshot(permissions, roles);
    }

    private static Guid? ReadUserId(ClaimsPrincipal? user)
    {
        var value = user?.FindFirstValue("uid") ?? user?.FindFirstValue(ClaimTypes.NameIdentifier);
        return Guid.TryParse(value, out var id) ? id : null;
    }

    private sealed record PermissionSnapshot(IReadOnlySet<string> Permissions, IReadOnlyList<string> Roles)
    {
        public static PermissionSnapshot Empty { get; } = new(
            new HashSet<string>(StringComparer.OrdinalIgnoreCase),
            []);
    }
}

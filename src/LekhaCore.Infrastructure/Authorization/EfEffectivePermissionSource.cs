using LekhaCore.Application.Interfaces.IRepo;

namespace LekhaCore.Infrastructure.Authorization;

public sealed class EfEffectivePermissionSource(IUserRoleRepository userRoles) : IEffectivePermissionSource
{
    public Task<IReadOnlyList<string>> GetPermissionCodesAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        return userRoles.GetPermissionCodesAsync(userId, cancellationToken);
    }

    public Task<IReadOnlyList<string>> GetRoleNamesAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        return userRoles.GetRoleNamesAsync(userId, cancellationToken);
    }
}

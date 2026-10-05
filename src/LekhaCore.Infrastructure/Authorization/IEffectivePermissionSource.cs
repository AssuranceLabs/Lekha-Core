namespace LekhaCore.Infrastructure.Authorization;

public interface IEffectivePermissionSource
{
    Task<IReadOnlyList<string>> GetPermissionCodesAsync(Guid userId, CancellationToken cancellationToken = default);

    Task<IReadOnlyList<string>> GetRoleNamesAsync(Guid userId, CancellationToken cancellationToken = default);
}

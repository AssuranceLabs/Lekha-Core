using LekhaCore.Domain.Common;

namespace LekhaCore.Application.Interfaces.IService;

public interface IRbacAdminService
{
    Task<Result> CreateRoleAsync(string name, CancellationToken cancellationToken = default);

    Task<Result> AssignRoleAsync(Guid userId, string roleName, CancellationToken cancellationToken = default);

    Task<Result> RemoveRoleAsync(Guid userId, string roleName, CancellationToken cancellationToken = default);

    Task<Result> GrantPermissionAsync(string roleName, string permissionCode, CancellationToken cancellationToken = default);

    Task<Result> RevokePermissionAsync(string roleName, string permissionCode, CancellationToken cancellationToken = default);
}

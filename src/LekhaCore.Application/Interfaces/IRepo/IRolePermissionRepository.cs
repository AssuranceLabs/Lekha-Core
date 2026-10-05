using LekhaCore.Domain.Entities;

namespace LekhaCore.Application.Interfaces.IRepo;

public interface IRolePermissionRepository : IRepository<RolePermission>
{
    Task<RolePermission?> GetTrackedAsync(string roleName, string permissionCode, CancellationToken cancellationToken = default);
}

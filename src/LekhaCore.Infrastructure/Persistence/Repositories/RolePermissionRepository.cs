using LekhaCore.Application.Interfaces.IRepo;
using LekhaCore.Domain.Entities;
using LekhaCore.Infrastructure.EntityFramework;
using Microsoft.EntityFrameworkCore;

namespace LekhaCore.Infrastructure.Persistence.Repositories;

public class RolePermissionRepository(AppDbContext context) : Repository<RolePermission>(context), IRolePermissionRepository
{
    public Task<RolePermission?> GetTrackedAsync(
        string roleName,
        string permissionCode,
        CancellationToken cancellationToken = default)
    {
        return Table
            .Include(grant => grant.Role)
            .Include(grant => grant.Permission)
            .FirstOrDefaultAsync(
                grant => grant.Role.Name == roleName && grant.Permission.Code == permissionCode,
                cancellationToken);
    }
}

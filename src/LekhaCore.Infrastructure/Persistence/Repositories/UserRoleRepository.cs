using LekhaCore.Application.Interfaces.IRepo;
using LekhaCore.Domain.Entities;
using LekhaCore.Infrastructure.EntityFramework;
using Microsoft.EntityFrameworkCore;

namespace LekhaCore.Infrastructure.Persistence.Repositories;

public class UserRoleRepository(AppDbContext context) : Repository<UserRole>(context), IUserRoleRepository
{
    public Task<UserRole?> GetTrackedAsync(Guid userId, string roleName, CancellationToken cancellationToken = default)
    {
        return Table
            .Include(assignment => assignment.Role)
            .Include(assignment => assignment.User)
            .FirstOrDefaultAsync(
                assignment => assignment.User.GUID == userId && assignment.Role.Name == roleName,
                cancellationToken);
    }

    public async Task<IReadOnlyList<string>> GetPermissionCodesAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        return await TableNoTracking
            .Where(assignment => assignment.User.GUID == userId && assignment.Role.IsActive)
            .SelectMany(assignment => assignment.Role.RolePermissions.Select(grant => grant.Permission.Code))
            .Distinct()
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<string>> GetRoleNamesAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        return await TableNoTracking
            .Where(assignment => assignment.User.GUID == userId && assignment.Role.IsActive)
            .Select(assignment => assignment.Role.Name)
            .Distinct()
            .OrderBy(name => name)
            .ToListAsync(cancellationToken);
    }
}

using LekhaCore.Domain.Entities;

namespace LekhaCore.Application.Interfaces.IRepo;

public interface IUserRoleRepository : IRepository<UserRole>
{
    Task<UserRole?> GetTrackedAsync(Guid userId, string roleName, CancellationToken cancellationToken = default);

    Task<IReadOnlyList<string>> GetPermissionCodesAsync(Guid userId, CancellationToken cancellationToken = default);

    Task<IReadOnlyList<string>> GetRoleNamesAsync(Guid userId, CancellationToken cancellationToken = default);
}

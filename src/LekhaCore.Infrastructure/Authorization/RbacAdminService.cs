using LekhaCore.Application.Interfaces;
using LekhaCore.Application.Interfaces.IRepo;
using LekhaCore.Application.Interfaces.IService;
using LekhaCore.Domain.Common;
using LekhaCore.Domain.Entities;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace LekhaCore.Infrastructure.Authorization;

public sealed class RbacAdminService(
    IUserRepository users,
    IRoleRepository roles,
    IPermissionRepository permissions,
    IUserRoleRepository userRoles,
    IRolePermissionRepository rolePermissions,
    IUnitOfWork unitOfWork,
    IPermissionService permissionService,
    ILogger<RbacAdminService> logger) : IRbacAdminService
{
    public async Task<Result> CreateRoleAsync(string name, CancellationToken cancellationToken = default)
    {
        var roleName = NormalizeName(name);
        if (roleName is null)
            return Result.Failure(Error.Validation("Rbac.RoleName", "Role name is required."));

        if (await roles.IsExistAsync(role => role.Name == roleName))
            return Result.Success();

        await roles.AddAsync(new Role
        {
            Name = roleName,
            IsActive = true
        });

        var saved = await SaveAsync(cancellationToken);
        if (saved.IsFailure)
            return saved;

        permissionService.Invalidate();
        logger.LogInformation("Security configuration changed. Role {RoleName} created.", roleName);
        return Result.Success();
    }

    public async Task<Result> AssignRoleAsync(Guid userId, string roleName, CancellationToken cancellationToken = default)
    {
        var normalizedRole = NormalizeName(roleName);
        if (normalizedRole is null)
            return Result.Failure(Error.Validation("Rbac.RoleName", "Role name is required."));

        var user = await users.GetByIdAsync(userId, cancellationToken);
        if (user is null)
            return Result.Failure(Error.NotFound("Rbac.UserNotFound", "User was not found."));

        var role = await roles.GetAsync(item => item.Name == normalizedRole && item.IsActive);
        if (role is null)
            return Result.Failure(Error.NotFound("Rbac.RoleNotFound", "Role was not found."));

        if (await userRoles.IsExistAsync(assignment => assignment.UserId == user.Id && assignment.RoleId == role.Id))
            return Result.Success();

        await userRoles.AddAsync(new UserRole
        {
            UserId = user.Id,
            RoleId = role.Id
        });

        var saved = await SaveAsync(cancellationToken);
        if (saved.IsFailure)
            return saved;

        permissionService.Invalidate();
        logger.LogInformation("Role {RoleName} assigned to user {UserId}.", normalizedRole, user.GUID);
        return Result.Success();
    }

    public async Task<Result> RemoveRoleAsync(Guid userId, string roleName, CancellationToken cancellationToken = default)
    {
        var normalizedRole = NormalizeName(roleName);
        if (normalizedRole is null)
            return Result.Failure(Error.Validation("Rbac.RoleName", "Role name is required."));

        var assignment = await userRoles.GetTrackedAsync(userId, normalizedRole, cancellationToken);
        if (assignment is null)
            return Result.Success();

        await userRoles.DeleteAsync(assignment);
        var saved = await SaveAsync(cancellationToken);
        if (saved.IsFailure)
            return saved;

        permissionService.Invalidate();
        logger.LogInformation("Role {RoleName} removed from user {UserId}.", normalizedRole, userId);
        return Result.Success();
    }

    public async Task<Result> GrantPermissionAsync(
        string roleName,
        string permissionCode,
        CancellationToken cancellationToken = default)
    {
        var normalizedRole = NormalizeName(roleName);
        var code = NormalizeCode(permissionCode);
        if (normalizedRole is null || code is null)
            return Result.Failure(Error.Validation("Rbac.Grant", "Role and permission are required."));

        var role = await roles.GetAsync(item => item.Name == normalizedRole);
        var permission = await permissions.GetAsync(item => item.Code == code);
        if (role is null || permission is null)
            return Result.Failure(Error.NotFound("Rbac.GrantNotFound", "Role or permission was not found."));

        if (await rolePermissions.IsExistAsync(grant => grant.RoleId == role.Id && grant.PermissionId == permission.Id))
            return Result.Success();

        await rolePermissions.AddAsync(new RolePermission
        {
            RoleId = role.Id,
            PermissionId = permission.Id
        });

        var saved = await SaveAsync(cancellationToken);
        if (saved.IsFailure)
            return saved;

        permissionService.Invalidate();
        logger.LogInformation("Permission {Permission} assigned to role {RoleName}.", code, normalizedRole);
        return Result.Success();
    }

    public async Task<Result> RevokePermissionAsync(
        string roleName,
        string permissionCode,
        CancellationToken cancellationToken = default)
    {
        var normalizedRole = NormalizeName(roleName);
        var code = NormalizeCode(permissionCode);
        if (normalizedRole is null || code is null)
            return Result.Failure(Error.Validation("Rbac.Revoke", "Role and permission are required."));

        var grant = await rolePermissions.GetTrackedAsync(normalizedRole, code, cancellationToken);
        if (grant is null)
            return Result.Success();

        await rolePermissions.DeleteAsync(grant);
        var saved = await SaveAsync(cancellationToken);
        if (saved.IsFailure)
            return saved;

        permissionService.Invalidate();
        logger.LogInformation("Permission {Permission} removed from role {RoleName}.", code, normalizedRole);
        return Result.Success();
    }

    private async Task<Result> SaveAsync(CancellationToken cancellationToken)
    {
        try
        {
            await unitOfWork.CommitAsync(cancellationToken: cancellationToken);
            return Result.Success();
        }
        catch (DbUpdateException exception) when (IsDuplicate(exception))
        {
            logger.LogInformation(exception, "RBAC relationship already exists.");
            permissionService.Invalidate();
            return Result.Success();
        }
    }

    private static bool IsDuplicate(DbUpdateException exception)
    {
        return exception.InnerException is SqlException sql && (sql.Number == 2601 || sql.Number == 2627);
    }

    private static string? NormalizeName(string? name)
    {
        if (string.IsNullOrWhiteSpace(name))
            return null;

        var trimmed = name.Trim();
        return trimmed.Length is > 0 and <= 100 ? trimmed : null;
    }

    private static string? NormalizeCode(string? code)
    {
        if (string.IsNullOrWhiteSpace(code))
            return null;

        return code.Trim().ToLowerInvariant();
    }
}

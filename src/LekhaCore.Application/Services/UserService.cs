using LekhaCore.Application.Common.Exceptions;
using LekhaCore.Application.DTOs.Users;
using LekhaCore.Application.Interfaces;
using LekhaCore.Application.Interfaces.IRepo;
using LekhaCore.Application.Interfaces.IService;
using LekhaCore.Domain.Common;
using LekhaCore.Domain.Common.Constants;
using LekhaCore.Domain.Entities;
using Microsoft.Extensions.Logging;

namespace LekhaCore.Application.Services;

public sealed class UserService(
    IUserRepository users,
    IRoleRepository roles,
    IUserRoleRepository userRoles,
    IUnitOfWork unitOfWork,
    IPasswordHasher passwordHasher,
    IPermissionService permissions,
    ILogger<UserService> logger) : IUserService
{
    public async Task<Result<UserDto>> CreateAsync(CreateUserRequest request, CancellationToken cancellationToken = default)
    {
        if (!HasRequiredPasswordShape(request.Password))
            return Result<UserDto>.Failure(Error.Validation("User.Password", ValidationMessages.PasswordWeak));

        var email = request.Email.Trim().ToLowerInvariant();
        if (await users.IsExistAsync(user => user.Email == email))
            return Result<UserDto>.Failure(Error.Conflict("User.EmailExists", "A user with this email already exists."));

        var roleNames = request.RoleNames
            .Where(name => !string.IsNullOrWhiteSpace(name))
            .Select(name => name.Trim())
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .ToList();

        var resolvedRoles = new List<Role>();
        foreach (var roleName in roleNames)
        {
            if (roleName.Length > 100)
                return Result<UserDto>.Failure(Error.Validation("User.RoleName", "Role name is too long."));

            var role = await roles.GetAsync(role => role.Name == roleName && role.IsActive);
            if (role is null)
                return Result<UserDto>.Failure(Error.NotFound("User.RoleNotFound", "Role was not found."));

            resolvedRoles.Add(role);
        }

        var user = new User
        {
            FullName = request.FullName.Trim(),
            Email = email,
            PasswordHash = passwordHasher.Hash(request.Password),
            IsActive = true
        };

        await users.AddAsync(user);
        foreach (var role in resolvedRoles)
        {
            await userRoles.AddAsync(new UserRole
            {
                User = user,
                RoleId = role.Id
            });
        }

        try
        {
            await unitOfWork.CommitAsync(cancellationToken: cancellationToken);
        }
        catch (UniqueConstraintException)
        {
            return Result<UserDto>.Failure(Error.Conflict("User.EmailExists", "A user with this email already exists."));
        }

        permissions.Invalidate();
        logger.LogInformation("User {UserId} created with {RoleCount} roles.", user.GUID, resolvedRoles.Count);

        return Result<UserDto>.Success(new UserDto
        {
            Id = user.GUID,
            FullName = user.FullName,
            Email = user.Email,
            IsActive = user.IsActive,
            Roles = resolvedRoles.Select(role => role.Name).OrderBy(name => name).ToList()
        });
    }

    private static bool HasRequiredPasswordShape(string password) =>
        password.Any(char.IsUpper) && password.Any(char.IsDigit);
}

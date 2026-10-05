using System.Security.Claims;
using LekhaCore.Application.Interfaces.IService;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Logging;

namespace LekhaCore.Infrastructure.Authorization;

public sealed class PermissionAuthorizationHandler(
    IPermissionService permissions,
    ILogger<PermissionAuthorizationHandler> logger) : AuthorizationHandler<PermissionRequirement>
{
    protected override async Task HandleRequirementAsync(
        AuthorizationHandlerContext context,
        PermissionRequirement requirement)
    {
        try
        {
            if (await permissions.HasPermissionAsync(context.User, requirement.Permission))
            {
                context.Succeed(requirement);
                return;
            }

            logger.LogWarning(
                "Authorization denied. User {UserId} does not have permission {Permission}.",
                UserId(context.User),
                requirement.Permission);
        }
        catch (Exception exception)
        {
            logger.LogError(
                exception,
                "Authorization failed closed for permission {Permission}. User {UserId}.",
                requirement.Permission,
                UserId(context.User));
        }
    }

    private static string UserId(ClaimsPrincipal user) =>
        user.FindFirstValue("uid")
        ?? user.FindFirstValue(ClaimTypes.NameIdentifier)
        ?? "unknown";
}

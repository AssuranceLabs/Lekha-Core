using LekhaCore.Application.DTOs.Dashboard;
using LekhaCore.Application.Interfaces.IRepo;
using LekhaCore.Application.Interfaces.IService;
using LekhaCore.Domain.Common;
using LekhaCore.Domain.Common.Constants;
using LekhaCore.Domain.Entities;

namespace LekhaCore.Application.Services;

public class MenuService(
    ICurrentUserService currentUser,
    IUserRepository users,
    IMenuRepository menus,
    IPermissionService permissions) : IMenuService
{
    private static readonly Error Unauthorized =
        Error.Unauthorized("Auth.Unauthorized", AuthMessages.Unauthorized);

    public async Task<Result<DashboardMenuResponse>> GetForCurrentUserAsync(CancellationToken cancellationToken = default)
    {
        if (currentUser.UserId is not Guid userId)
            return Result<DashboardMenuResponse>.Failure(Unauthorized);

        var user = await users.GetByPublicIdReadOnlyAsync(userId, cancellationToken);
        if (user is null || !user.IsActive)
            return Result<DashboardMenuResponse>.Failure(Unauthorized);

        var granted = await permissions.GetPermissionsAsync(userId, cancellationToken);
        var activeMenus = await menus.ListActiveAsync(cancellationToken);
        var grantedIds = activeMenus
            .Where(menu => menu.RequiredPermission is not null && granted.Contains(menu.RequiredPermission))
            .Select(menu => menu.Id)
            .ToHashSet();

        return Result<DashboardMenuResponse>.Success(new DashboardMenuResponse
        {
            Permissions = granted,
            Menus = BuildTree(activeMenus, grantedIds)
        });
    }

    private static IReadOnlyList<DashboardMenuItemDto> BuildTree(
        IReadOnlyList<Menu> activeMenus,
        HashSet<int> grantedIds)
    {
        var byId = activeMenus.ToDictionary(menu => menu.Id);
        var included = new HashSet<int>();

        foreach (var grantedId in grantedIds)
        {
            if (!byId.TryGetValue(grantedId, out var current))
                continue;

            while (true)
            {
                if (!included.Add(current.Id))
                    break;

                if (current.ParentId is not int parentId || !byId.TryGetValue(parentId, out var parent))
                    break;

                current = parent;
            }
        }

        var visible = activeMenus.Where(menu => included.Contains(menu.Id)).ToList();

        bool ParentVisible(Menu menu) =>
            menu.ParentId is int parentId && included.Contains(parentId);

        List<DashboardMenuItemDto> BuildChildren(int parentId) =>
            visible
                .Where(menu => menu.ParentId == parentId)
                .OrderBy(menu => menu.SortOrder)
                .ThenBy(menu => menu.Name)
                .Select(menu => ToDto(menu, grantedIds, BuildChildren(menu.Id)))
                .ToList();

        return visible
            .Where(menu => !ParentVisible(menu))
            .OrderBy(menu => menu.SortOrder)
            .ThenBy(menu => menu.Name)
            .Select(menu => ToDto(menu, grantedIds, BuildChildren(menu.Id)))
            .ToList();
    }

    private static DashboardMenuItemDto ToDto(
        Menu menu,
        HashSet<int> grantedIds,
        IReadOnlyList<DashboardMenuItemDto> children)
    {
        var isAccessible = grantedIds.Contains(menu.Id);
        return new DashboardMenuItemDto
        {
            Code = menu.Code,
            Name = menu.Name,
            Route = isAccessible ? menu.Route : null,
            Icon = menu.Icon,
            SortOrder = menu.SortOrder,
            IsAccessible = isAccessible,
            Children = children
        };
    }
}

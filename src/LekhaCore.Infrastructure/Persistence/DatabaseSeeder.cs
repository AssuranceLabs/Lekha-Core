using LekhaCore.Domain.Authorization;
using LekhaCore.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace LekhaCore.Infrastructure.Persistence;

public static class DatabaseSeeder
{
    public static async Task SeedAsync(AppDbContext context, IConfiguration configuration)
    {
        await SeedPermissionsAsync(context);
        await SeedRolesAsync(context, configuration);
        await SeedMenusAsync(context);
    }

    private static async Task SeedPermissionsAsync(AppDbContext context)
    {
        var existing = await context.Permissions
            .IgnoreQueryFilters()
            .Select(permission => permission.Code)
            .ToListAsync();
        var known = existing.ToHashSet(StringComparer.OrdinalIgnoreCase);

        foreach (var code in Permissions.All)
        {
            if (!known.Add(code))
                continue;

            context.Permissions.Add(new Permission
            {
                Code = code,
                Name = code
            });
        }

        await context.SaveChangesAsync();
    }

    private static async Task SeedRolesAsync(AppDbContext context, IConfiguration configuration)
    {
        var seeds = configuration.GetSection("Rbac:Roles").Get<List<RbacRoleSeed>>() ?? [];
        var permissions = await context.Permissions.ToListAsync();
        var permissionByCode = permissions.ToDictionary(permission => permission.Code, StringComparer.OrdinalIgnoreCase);
        var roles = await context.Roles.ToListAsync();
        var roleByName = roles.ToDictionary(role => role.Name, StringComparer.OrdinalIgnoreCase);
        var existingLinks = await context.RolePermissions
            .Select(grant => new { grant.RoleId, grant.Permission.Code })
            .ToListAsync();
        var linkSet = existingLinks
            .Select(link => (link.RoleId, link.Code.ToLowerInvariant()))
            .ToHashSet();

        foreach (var seed in seeds)
        {
            var name = seed.Name.Trim();
            if (string.IsNullOrWhiteSpace(name))
                throw new InvalidOperationException("RBAC seed role name is required.");

            if (!roleByName.TryGetValue(name, out var role))
            {
                role = new Role
                {
                    Name = name,
                    Description = seed.Description,
                    IsActive = true
                };
                context.Roles.Add(role);
                roleByName[name] = role;
            }

            IEnumerable<string> codes = seed.GrantAllDefinedPermissions
                ? Permissions.All
                : seed.Permissions;

            foreach (var code in codes.Distinct(StringComparer.OrdinalIgnoreCase))
            {
                if (!Permissions.IsDefined(code))
                    throw new InvalidOperationException($"Unknown permission '{code}' on role '{name}'.");

                if (!permissionByCode.TryGetValue(code, out var permission))
                    throw new InvalidOperationException($"Permission '{code}' was not seeded.");

                var key = (role.Id, code.ToLowerInvariant());
                if (role.Id > 0 && !linkSet.Add(key))
                    continue;

                if (role.Id == 0 && role.RolePermissions.Any(grant =>
                        string.Equals(grant.Permission.Code, code, StringComparison.OrdinalIgnoreCase)))
                    continue;

                var grant = new RolePermission
                {
                    Role = role,
                    Permission = permission
                };
                role.RolePermissions.Add(grant);
                context.RolePermissions.Add(grant);
            }
        }

        await context.SaveChangesAsync();
    }

    private static async Task SeedMenusAsync(AppDbContext context)
    {
        var existing = await context.Menus.IgnoreQueryFilters().ToListAsync();
        var byCode = existing.ToDictionary(menu => menu.Code, StringComparer.OrdinalIgnoreCase);

        foreach (var definition in DashboardMenuCatalog.All.Where(menu => menu.ParentCode is null))
            AddOrUpdateMenu(context, byCode, definition, parent: null);

        foreach (var definition in DashboardMenuCatalog.All.Where(menu => menu.ParentCode is not null))
        {
            if (!byCode.TryGetValue(definition.ParentCode!, out var parent))
                throw new InvalidOperationException($"Parent menu '{definition.ParentCode}' was not seeded.");

            AddOrUpdateMenu(context, byCode, definition, parent);
        }

        var catalogCodes = DashboardMenuCatalog.All
            .Select(menu => menu.Code)
            .ToHashSet(StringComparer.OrdinalIgnoreCase);
        foreach (var menu in existing.Where(menu => !menu.IsDeleted && !catalogCodes.Contains(menu.Code)))
            context.Menus.Remove(menu);

        await context.SaveChangesAsync();
    }

    private static void AddOrUpdateMenu(
        AppDbContext context,
        Dictionary<string, Menu> byCode,
        MenuSeedDefinition definition,
        Menu? parent)
    {
        if (byCode.TryGetValue(definition.Code, out var existing))
        {
            existing.RequiredPermission = definition.RequiredPermission;
            existing.Name = definition.Name;
            existing.Route = definition.Route;
            existing.Icon = definition.Icon;
            existing.SortOrder = definition.SortOrder;
            existing.IsActive = true;
            existing.IsDeleted = false;
            existing.DeletedBy = null;
            existing.DeletedOn = null;
            return;
        }

        var menu = new Menu
        {
            Code = definition.Code,
            Name = definition.Name,
            Route = definition.Route,
            Icon = definition.Icon,
            SortOrder = definition.SortOrder,
            IsActive = true,
            RequiredPermission = definition.RequiredPermission
        };

        if (parent is { Id: > 0 })
            menu.ParentId = parent.Id;
        else if (parent is not null)
            menu.Parent = parent;

        context.Menus.Add(menu);
        byCode[definition.Code] = menu;
    }
}

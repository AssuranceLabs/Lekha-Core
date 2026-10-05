namespace LekhaCore.Infrastructure.Persistence;

public sealed record MenuSeedDefinition(
    string Code,
    string Name,
    string? Route,
    string? Icon,
    int SortOrder,
    string? ParentCode,
    string? RequiredPermission);

public static class DashboardMenuCatalog
{
    public static IReadOnlyList<MenuSeedDefinition> All { get; } =
    [
        Item("dashboard", "Dashboard", "/dashboard", "layout-dashboard", 10, null, "dashboard.view"),
        Item("engagements", "Engagements", "/engagements", "briefcase", 20, null, "engagements.view"),
        Item("engagements.active", "Active engagements", "/engagements/active", "list", 10, "engagements", "engagements.view"),
        Item("engagements.planning", "Planning", "/engagements/planning", "calendar", 20, "engagements", "engagements.view"),
        Item("clients", "Clients", "/clients", "users", 30, null, "clients.view"),
        Item("workpapers", "Workpapers", "/workpapers", "file-text", 40, null, "workpapers.view"),
        Item("quality", "Quality review", "/quality", "shield-check", 50, null, "quality.view"),
        Item("reports", "Reports", "/reports", "chart", 60, null, "reports.view"),
        Item("notifications", "Notifications", "/notifications", "bell", 70, null, "notifications.view"),
        Item("administration", "Administration", null, "settings", 80, null, "administration.view"),
        Item("administration.users", "Users", "/admin/users", "user-cog", 10, "administration", "users.create"),
        Item("administration.audit", "Audit log", "/admin/audit", "scroll-text", 20, "administration", "audit.view"),
        Item("administration.settings", "Settings", "/admin/settings", "sliders", 30, "administration", "settings.view")
    ];

    private static MenuSeedDefinition Item(
        string code,
        string name,
        string? route,
        string? icon,
        int sortOrder,
        string? parentCode,
        string? requiredPermission) =>
        new(code, name, route, icon, sortOrder, parentCode, requiredPermission);
}

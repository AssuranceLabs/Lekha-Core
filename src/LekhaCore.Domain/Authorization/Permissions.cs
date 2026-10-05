namespace LekhaCore.Domain.Authorization;

public static class Permissions
{
    public const string DashboardView = "dashboard.view";
    public const string EngagementsView = "engagements.view";
    public const string ClientsView = "clients.view";
    public const string WorkpapersView = "workpapers.view";
    public const string QualityView = "quality.view";
    public const string ReportsView = "reports.view";
    public const string NotificationsView = "notifications.view";
    public const string AdministrationView = "administration.view";
    public const string UsersCreate = "users.create";
    public const string AuditView = "audit.view";
    public const string SettingsView = "settings.view";
    public const string SettingsManage = "settings.manage";

    public static readonly IReadOnlySet<string> All = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
    {
        DashboardView,
        EngagementsView,
        ClientsView,
        WorkpapersView,
        QualityView,
        ReportsView,
        NotificationsView,
        AdministrationView,
        UsersCreate,
        AuditView,
        SettingsView,
        SettingsManage
    };

    public static bool IsDefined(string? code) =>
        !string.IsNullOrWhiteSpace(code) && All.Contains(code);
}

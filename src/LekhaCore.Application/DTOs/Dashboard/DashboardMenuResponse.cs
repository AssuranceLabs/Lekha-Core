namespace LekhaCore.Application.DTOs.Dashboard;

public sealed class DashboardMenuResponse
{
    public IReadOnlySet<string> Permissions { get; set; } = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

    public IReadOnlyList<DashboardMenuItemDto> Menus { get; set; } = [];
}

public sealed class DashboardMenuItemDto
{
    public string Code { get; set; } = string.Empty;

    public string Name { get; set; } = string.Empty;

    public string? Route { get; set; }

    public string? Icon { get; set; }

    public int SortOrder { get; set; }

    public bool IsAccessible { get; set; }

    public IReadOnlyList<DashboardMenuItemDto> Children { get; set; } = [];
}

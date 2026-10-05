namespace LekhaCore.Infrastructure.Persistence;

public sealed class RbacRoleSeed
{
    public string Name { get; set; } = string.Empty;

    public string? Description { get; set; }

    public bool GrantAllDefinedPermissions { get; set; }

    public List<string> Permissions { get; set; } = [];
}

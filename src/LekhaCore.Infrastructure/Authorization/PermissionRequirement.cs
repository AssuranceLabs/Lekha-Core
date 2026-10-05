using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Authorization;

namespace LekhaCore.Infrastructure.Authorization;

public sealed class PermissionRequirement(string permission) : IAuthorizationRequirement
{
    public string Permission { get; } = permission;
}

public static partial class PermissionPolicyNames
{
    public static bool IsPermissionPolicy(string? policyName) =>
        !string.IsNullOrWhiteSpace(policyName) && PermissionName().IsMatch(policyName);

    [GeneratedRegex("^[a-z][a-z0-9_]*(?:\\.[a-z][a-z0-9_]*)+$", RegexOptions.CultureInvariant)]
    private static partial Regex PermissionName();
}

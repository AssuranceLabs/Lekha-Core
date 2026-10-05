namespace LekhaCore.Application.Authorization;

public static class EffectivePermissions
{
    public static IReadOnlySet<string> Union(IEnumerable<string>? codes)
    {
        var set = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        if (codes is null)
            return set;

        foreach (var code in codes)
        {
            if (!string.IsNullOrWhiteSpace(code))
                set.Add(code.Trim());
        }

        return set;
    }
}

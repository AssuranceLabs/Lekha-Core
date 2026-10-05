using LekhaCore.Core.Domain.Common;

namespace LekhaCore.Domain.Entities;

[AuditSubType("RefreshToken")]
public class RefreshToken : IAuditableEntity
{
    public Guid Id { get; set; }

    public string TokenHash { get; set; } = string.Empty;

    public Guid UserId { get; set; }

    public DateTime Created { get; set; }

    public DateTime Expires { get; set; }

    public bool IsRevoked { get; set; }

    public DateTime? RevokedAt { get; set; }

    public string? ReplacedByTokenHash { get; set; }

    public string? CreatedByIp { get; set; }
}

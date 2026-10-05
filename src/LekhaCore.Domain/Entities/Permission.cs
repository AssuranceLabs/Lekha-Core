using LekhaCore.Core.BaseEntity;
using LekhaCore.Core.Domain.Common;
using LekhaCore.Domain.Common.Constants;

namespace LekhaCore.Domain.Entities;

[AuditSubType("Permission")]
public class Permission : IAuditableEntity, IFullAudited
{
    public int Id { get; set; }

    public string Code { get; set; } = string.Empty;

    public string Name { get; set; } = string.Empty;

    public ICollection<RolePermission> RolePermissions { get; set; } = new List<RolePermission>();

    public string CreatedBy { get; set; } = AuditActors.System;

    public DateTime CreatedOn { get; set; }

    public string? ModifiedBy { get; set; }

    public DateTime? ModifiedOn { get; set; }

    public bool IsDeleted { get; set; }

    public string? DeletedBy { get; set; }

    public DateTime? DeletedOn { get; set; }
}

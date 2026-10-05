using LekhaCore.Core.BaseEntity;
using LekhaCore.Core.Domain.Common;
using LekhaCore.Domain.Common.Constants;

namespace LekhaCore.Domain.Entities;

[AuditSubType("RolePermission")]
public class RolePermission : IAuditableEntity, IFullAudited
{
    public int Id { get; set; }

    public int RoleId { get; set; }

    public Role Role { get; set; } = null!;

    public int PermissionId { get; set; }

    public Permission Permission { get; set; } = null!;

    public string CreatedBy { get; set; } = AuditActors.System;

    public DateTime CreatedOn { get; set; }

    public string? ModifiedBy { get; set; }

    public DateTime? ModifiedOn { get; set; }

    public bool IsDeleted { get; set; }

    public string? DeletedBy { get; set; }

    public DateTime? DeletedOn { get; set; }
}

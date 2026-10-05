using LekhaCore.Core.BaseEntity;
using LekhaCore.Core.Domain.Common;
using LekhaCore.Domain.Common.Constants;

namespace LekhaCore.Domain.Entities;

[AuditSubType("Menu")]
public class Menu : IAuditableEntity, IFullAudited
{
    public int Id { get; set; }

    public string Code { get; set; } = string.Empty;

    public string Name { get; set; } = string.Empty;

    public string? Route { get; set; }

    public string? Icon { get; set; }

    public int? ParentId { get; set; }

    public Menu? Parent { get; set; }

    public ICollection<Menu> Children { get; set; } = new List<Menu>();

    public int SortOrder { get; set; }

    public string? RequiredPermission { get; set; }

    public bool IsActive { get; set; } = true;

    public string CreatedBy { get; set; } = AuditActors.System;

    public DateTime CreatedOn { get; set; }

    public string? ModifiedBy { get; set; }

    public DateTime? ModifiedOn { get; set; }

    public bool IsDeleted { get; set; }

    public string? DeletedBy { get; set; }

    public DateTime? DeletedOn { get; set; }
}

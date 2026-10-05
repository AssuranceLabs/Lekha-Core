using LekhaCore.Core.BaseEntity;
using LekhaCore.Core.Domain.Common;
using LekhaCore.Domain.Common.Constants;

namespace LekhaCore.Domain.Common;

public abstract class BaseEntity : IEntity<Guid>, IFullAudited, IAuditableEntity
{
    public Guid Id { get; set; } = Guid.NewGuid();

    public string CreatedBy { get; set; } = AuditActors.System;

    public DateTime CreatedOn { get; set; }

    public string? ModifiedBy { get; set; }

    public DateTime? ModifiedOn { get; set; }

    public bool IsDeleted { get; set; }

    public string? DeletedBy { get; set; }

    public DateTime? DeletedOn { get; set; }
}

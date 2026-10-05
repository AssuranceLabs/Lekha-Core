using LekhaCore.Core.BaseEntity;
using LekhaCore.Core.Domain.Common;
using LekhaCore.Domain.Common.Constants;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LekhaCore.Domain.Entities;

[AuditSubType("User")]
[Table("Users")]
public class User : IAuditableEntity, IFullAudited
{
    [Required]
    [DatabaseGenerated(DatabaseGeneratedOption.None)]
    public Guid GUID { get; set; } = Guid.NewGuid();

    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }

    [Required]
    public string FullName { get; set; } = string.Empty;

    [Required]
    public string Email { get; set; } = string.Empty;

    [Required]
    public string PasswordHash { get; set; } = string.Empty;

    public bool IsActive { get; set; } = true;

    public int FailedLoginAttempts { get; set; }

    public DateTime? LockoutEndUtc { get; set; }

    public DateTime? LastLoginAt { get; set; }

    [Required]
    [MaxLength(64)]
    public string CreatedBy { get; set; } = AuditActors.System;

    [Required]
    public DateTime CreatedOn { get; set; }

    [MaxLength(64)]
    public string? ModifiedBy { get; set; }

    public DateTime? ModifiedOn { get; set; }

    public bool IsDeleted { get; set; }

    [MaxLength(64)]
    public string? DeletedBy { get; set; }

    public DateTime? DeletedOn { get; set; }
}

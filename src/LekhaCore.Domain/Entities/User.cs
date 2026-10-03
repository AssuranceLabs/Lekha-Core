using LekhaCore.Core.BaseEntity;
using LekhaCore.Domain.Common;
using LekhaCore.Domain.Enums;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LekhaCore.Domain.Entities;

[Table("Users")]
public class User : IAuditableEntity, IFullAudited
{

    [Required]
    [DatabaseGenerated(DatabaseGeneratedOption.None)]
    public Guid GUID { get; set; }

    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }


    [Required]
    public string FullName { get; set; } = string.Empty;

    [Required]
    public string Email { get; set; } = string.Empty;

    [Required]
    public string PasswordHash { get; set; } = string.Empty;

    public Role Role { get; set; }

    [Required]
    [MaxLength(50)]
    public string CreatedBy { get; set; }

    [Required]
    public DateTime CreatedOn { get; set; }

    [MaxLength(50)]
    public string? ModifiedBy { get; set; }

    public DateTime? ModifiedOn { get; set; }

    public bool IsDeleted { get; set; }

    [MaxLength(50)]
    public string? DeletedBy { get; set; }

    public DateTime? DeletedOn { get; set; }
}
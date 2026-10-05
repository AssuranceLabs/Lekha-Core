using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LekhaCore.Core.Domain.Common;

[Table("AuditLogs")]
public class AuditLog
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }

    [Required]
    [StringLength(50)]
    public string UserId { get; set; } = string.Empty;

    [Required]
    [StringLength(50)]
    public string Type { get; set; } = string.Empty;

    [StringLength(255)]
    public string? SubType { get; set; }

    [Required]
    [StringLength(255)]
    public string TableName { get; set; } = string.Empty;

    [Required]
    public DateTime DateTime { get; set; }

    public string? OldValues { get; set; }

    public string? NewValues { get; set; }

    public string? AffectedColumns { get; set; }

    [Required]
    [StringLength(256)]
    public string PrimaryKey { get; set; } = string.Empty;

    [StringLength(64)]
    public string? IpAddress { get; set; }

    [StringLength(100)]
    public string? CorrelationId { get; set; }
}

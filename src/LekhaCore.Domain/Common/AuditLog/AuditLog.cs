using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LekhaCore.Core.Domain.Common
{
    [Table("AuditLog")]
    public class AuditLog
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        [StringLength(50)]
        public string UserId { get; set; }

        [Required]
        [StringLength(50)]
        public string Type { get; set; }

        [StringLength(255)]
        public string? SubType { get; set; }

        [Required]
        [StringLength(255)]
        public string TableName { get; set; }

        [Required]
        public DateTime DateTime { get; set; }

        public string? OldValues { get; set; }

        public string? NewValues { get; set; }

        public string? AffectedColumns { get; set; }

        public string PrimaryKey { get; set; }
    }
}
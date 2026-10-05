using LekhaCore.Core.Domain.Common;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LekhaCore.Infrastructure.Persistence.Configurations;

public class AuditLogConfiguration : IEntityTypeConfiguration<AuditLog>
{
    public void Configure(EntityTypeBuilder<AuditLog> builder)
    {
        builder.ToTable("AuditLogs");
        builder.HasKey(x => x.Id);

        builder.Property(x => x.UserId).HasMaxLength(50).IsRequired();
        builder.Property(x => x.Type).HasMaxLength(50).IsRequired();
        builder.Property(x => x.SubType).HasMaxLength(255);
        builder.Property(x => x.TableName).HasMaxLength(255).IsRequired();
        builder.Property(x => x.PrimaryKey).HasMaxLength(256).IsRequired();
        builder.Property(x => x.IpAddress).HasMaxLength(64);
        builder.Property(x => x.CorrelationId).HasMaxLength(100);
        builder.Property(x => x.DateTime).IsRequired();

        builder.HasIndex(x => new { x.TableName, x.DateTime });
        builder.HasIndex(x => new { x.UserId, x.DateTime });
    }
}

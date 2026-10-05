using LekhaCore.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LekhaCore.Infrastructure.Persistence.Configurations;

public class MenuConfiguration : IEntityTypeConfiguration<Menu>
{
    public void Configure(EntityTypeBuilder<Menu> builder)
    {
        builder.ToTable("Menus", table =>
            table.HasCheckConstraint("CK_Menus_ParentNotSelf", "[ParentId] IS NULL OR [ParentId] <> [Id]"));

        builder.HasKey(x => x.Id);
        builder.ConfigureAudit();

        builder.Property(x => x.Code).HasMaxLength(100).IsRequired();
        builder.HasIndex(x => x.Code).IsUnique().HasFilter("[IsDeleted] = 0");

        builder.Property(x => x.Name).HasMaxLength(150).IsRequired();
        builder.Property(x => x.Route).HasMaxLength(300);
        builder.Property(x => x.Icon).HasMaxLength(100);
        builder.Property(x => x.RequiredPermission).HasMaxLength(128);
        builder.Property(x => x.IsActive).IsRequired().HasDefaultValue(true);
        builder.HasIndex(x => x.ParentId);

        builder.HasOne(x => x.Parent)
            .WithMany(x => x.Children)
            .HasForeignKey(x => x.ParentId);
    }
}

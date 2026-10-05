using LekhaCore.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LekhaCore.Infrastructure.Persistence.Configurations;

public class RoleConfiguration : IEntityTypeConfiguration<Role>
{
    public void Configure(EntityTypeBuilder<Role> builder)
    {
        builder.ToTable("Roles");
        builder.HasKey(role => role.Id);
        builder.ConfigureAudit();
        builder.Property(role => role.Name).HasMaxLength(100).IsRequired();
        builder.Property(role => role.Description).HasMaxLength(500);
        builder.Property(role => role.IsActive).IsRequired().HasDefaultValue(true);
        builder.HasIndex(role => role.Name).IsUnique().HasFilter("[IsDeleted] = 0");
    }
}

public class PermissionConfiguration : IEntityTypeConfiguration<Permission>
{
    public void Configure(EntityTypeBuilder<Permission> builder)
    {
        builder.ToTable("Permissions");
        builder.HasKey(permission => permission.Id);
        builder.ConfigureAudit();
        builder.Property(permission => permission.Code).HasMaxLength(128).IsRequired();
        builder.Property(permission => permission.Name).HasMaxLength(200).IsRequired();
        builder.HasIndex(permission => permission.Code).IsUnique().HasFilter("[IsDeleted] = 0");
    }
}

public class UserRoleConfiguration : IEntityTypeConfiguration<UserRole>
{
    public void Configure(EntityTypeBuilder<UserRole> builder)
    {
        builder.ToTable("UserRoles");
        builder.HasKey(assignment => assignment.Id);
        builder.ConfigureAudit();
        builder.HasIndex(assignment => assignment.UserId);
        builder.HasIndex(assignment => assignment.RoleId);
        builder.HasIndex(assignment => new { assignment.UserId, assignment.RoleId })
            .IsUnique()
            .HasFilter("[IsDeleted] = 0");

        builder.HasOne(assignment => assignment.User)
            .WithMany()
            .HasForeignKey(assignment => assignment.UserId)
            .IsRequired();

        builder.HasOne(assignment => assignment.Role)
            .WithMany(role => role.UserRoles)
            .HasForeignKey(assignment => assignment.RoleId)
            .IsRequired();
    }
}

public class RolePermissionConfiguration : IEntityTypeConfiguration<RolePermission>
{
    public void Configure(EntityTypeBuilder<RolePermission> builder)
    {
        builder.ToTable("RolePermissions");
        builder.HasKey(grant => grant.Id);
        builder.ConfigureAudit();
        builder.HasIndex(grant => grant.RoleId);
        builder.HasIndex(grant => grant.PermissionId);
        builder.HasIndex(grant => new { grant.RoleId, grant.PermissionId })
            .IsUnique()
            .HasFilter("[IsDeleted] = 0");

        builder.HasOne(grant => grant.Role)
            .WithMany(role => role.RolePermissions)
            .HasForeignKey(grant => grant.RoleId)
            .IsRequired();

        builder.HasOne(grant => grant.Permission)
            .WithMany(permission => permission.RolePermissions)
            .HasForeignKey(grant => grant.PermissionId)
            .IsRequired();
    }
}
